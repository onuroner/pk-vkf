namespace Vk.Schema;

public class AccountRequest
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string CurrencyCode { get; set; }
}

public class AccountResponse 
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Name { get; set; }
    public int AccountNumber { get; set; }
    public string IBAN { get; set; }
    public decimal Balance { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public int? CardId { get; set; }
    
    public virtual List<EftTransactionResponse> EftTransactions { get; set; }
    public virtual List<AccountTransactionResponse> AccountTransactions { get; set; }
}