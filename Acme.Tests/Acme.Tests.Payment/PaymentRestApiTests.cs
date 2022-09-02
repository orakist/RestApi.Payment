using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Acme.Foundation.Application.Dtos;
using Acme.Payment.Application.Dtos;
using Acme.Payment.RestApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Acme.Tests.Payment;

public class PaymentRestApiTests
{
    private readonly HttpClient _client;

    public PaymentRestApiTests()
    {
        var appFactory = new WebApplicationFactory<Program>();
        _client = appFactory.CreateClient();
    }

    [Fact]
    public async Task GetAccountList()
    {
        //arrange
        var response = await _client.GetAsync("api/account/list");

        //act
        var responseContent = await response.Content.ReadAsStringAsync();
        var accounts = JsonConvert.DeserializeObject<PagedResultDto<AccountSimpleDto>>(responseContent);

        //assert
        Assert.Equal(3, accounts.TotalCount);
        Assert.NotNull(accounts.Items.FirstOrDefault(a => a.AccountNumber == 4755));
    }

    [Fact]
    public async Task TestPaymentTransaction()
    {
        //arrange
        var transactionRequest = new TransactionPaymentDto
        { AccountNumber = 4755, Amount = 100, MessageType = "PAYMENT", Origin = "MASTER" };

        var response = await _client.PostAsJsonAsync("api/transaction/payment", transactionRequest);

        //act
        var responseContent = await response.Content.ReadAsStringAsync();
        var resultTransaction = JsonConvert.DeserializeObject<TransactionDto>(responseContent);

        //assert
        Assert.Equal(4755, resultTransaction.Account.AccountNumber);
        Assert.Equal(100, resultTransaction.Amount);
        Assert.Equal(899.88m, resultTransaction.Account.Balance);
    }


    [Fact]
    public async Task TestPaymentThenAdjustmentTransaction()
    {
        //arrange
        var paymentRequest = new TransactionPaymentDto
        { AccountNumber = 4755, Amount = 100, MessageType = "PAYMENT", Origin = "MASTER" };
        var adjustmentRequest = new TransactionPaymentDto
        { AccountNumber = 4755, Amount = 80, MessageType = "ADJUSTMENT", Origin = "VISA" };

        //act
        var paymentResponse = await _client.PostAsJsonAsync("api/transaction/payment", paymentRequest);
        var firstResult = JsonConvert.DeserializeObject<TransactionDto>(await paymentResponse.Content.ReadAsStringAsync());

        adjustmentRequest.TransactionId = firstResult.Id;
        var adjustmentResponse = await _client.PostAsJsonAsync("api/transaction/payment", adjustmentRequest);
        var secondResult = JsonConvert.DeserializeObject<TransactionDto>(await adjustmentResponse.Content.ReadAsStringAsync());

        //assert
        Assert.Equal(4755, firstResult.Account.AccountNumber);
        Assert.Equal(100, firstResult.Amount);
        Assert.Equal(899.88m, firstResult.Account.Balance);

        Assert.Equal(firstResult.Id, secondResult.Id);
        Assert.Equal(921.08m, secondResult.Account.Balance);
    }
}