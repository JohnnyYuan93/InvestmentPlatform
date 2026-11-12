using System;
    
namespace InvestmentPlatform.Domain.Exception;

public class InsufficientFundsException(string message) : System.Exception(message)
{
}