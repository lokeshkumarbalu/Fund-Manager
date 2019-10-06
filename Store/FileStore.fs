namespace FundManager.Store

open System
open FSharp.Data
open FundManager.Domain
open FundManager.Logging
open System.IO

type FileStore (directoryInfo: DirectoryInfo, log: ILog) =
    let onError ex = Common.onError log ex

    let extractPersonDetails (row: CsvProvider<Constants.CsvStructure.Person>.Row) = 
        { Id = 0
          Name = row.Name
          StartDate = row.StartDate.ToUniversalTime()
          Status = PersonAttributes.Status.fromString(row.Status) }

    let extractTransactionDetails (row: CsvProvider<Constants.CsvStructure.Transaction>.Row) = 
        { Id = 0
          Type = TransactionAttributes.Type.fromString(row.Category)
          Amount = row.Amount
          Date = row.Date.ToUniversalTime()
          Currency = Currency.fromString(row.Currency)
          Description = row.Description }

    let extractFundDetails (row: CsvProvider<Constants.CsvStructure.Fund>.Row) =
        { Id = 0
          YearMonth = row.YearMonth
          Contributor = row.Contributor
          Amount = row.Amount
          Comment = row.Comment }

    
    (* Exposed methods here *)
    
    member __.GetFunds() =
        try
            let csvFileFunds = directoryInfo.ToString() + Constants.CsvFile.Funds
            if File.Exists csvFileFunds then
                log.Info "Fetching fund list from file: '%s'" csvFileFunds
                let data = CsvProvider<Constants.CsvStructure.Fund>.Load(csvFileFunds)

                data.Rows 
                |> Seq.map extractFundDetails 
                |> Seq.toList
                |> Ok
            else
                log.Error "File missing: '%s'" csvFileFunds
                Error "The specified file does not exist, aborting fetch operation"
        with 
        | ex -> onError ex

    member __.GetPeople() = 
        try 
            let csvFilePeople = directoryInfo.ToString() + Constants.CsvFile.People
            if File.Exists csvFilePeople then 
                log.Info "Fetching people list from file: '%s'" csvFilePeople
                let data = CsvProvider<Constants.CsvStructure.Person>.Load(csvFilePeople)

                data.Rows 
                |> Seq.map extractPersonDetails 
                |> Seq.toList
                |> Ok
            else 
                log.Error "File missing: '%s'" csvFilePeople
                Error "The specified file does not exist, aborting fetch operation"
        with 
        | ex -> onError ex 

    member __.GetTransactions() = 
        try 
            let csvFileTransactions = directoryInfo.ToString() + Constants.CsvFile.Transactions
            if File.Exists csvFileTransactions then 
                log.Info "Fetching transaction list from file: '%s'" csvFileTransactions
                let data = CsvProvider<Constants.CsvStructure.Transaction>.Load(csvFileTransactions) 
                
                data.Rows 
                |> Seq.map extractTransactionDetails 
                |> Seq.toList
                |> Ok
            else
                log.Error "File missing: '%s'" csvFileTransactions
                Error "The specified file does not exist, aborting fetch operation"
        with 
        | ex -> onError ex

    
