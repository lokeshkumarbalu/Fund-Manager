open FundManager.Application
open FundManager.Application.BusinessLogic
open FundManager.Logging
open FundManager.Store
open System.IO
open FundManager.Store

[<EntryPoint>]
let main argv = 
    let log = CompositeLog(Constants.ApplciationLoggerName) :> ILog
    let dbLog = CompositeLog(Constants.DbStoreLoggerName) :> ILog
    let dataTransferDirectory = new DirectoryInfo(Constants.DataTransferPath)
    let fileStore = FileStore(dataTransferDirectory, CompositeLog(Constants.FileStoreLoggerName))
    use fundStore = new FundStore(dataTransferDirectory, dbLog)
    use transactionStore = new TransactionStore(dataTransferDirectory, dbLog)
    use personStore = new PersonStore(dataTransferDirectory, dbLog)
    
    DataLoader.loadPeople personStore fileStore |> log.Info "%A"
    DataLoader.loadTransactions transactionStore fileStore |> log.Info "%A"
    DataLoader.loadFunds fundStore fileStore |> log.Info "%A"

    FundProcessor.getBalance transactionStore fundStore
    |> Result.map (FundProcessor.printBalance log)
    |> ignore

    0
        