open FundManager.Application
open FundManager.Application.BusinessLogic
open FundManager.Logging
open FundManager.Store
open System.IO

let printBalance (log: ILog) tuple =
    log.Info "| %-20s | %8s | %8s | %8s |" "CONTRIBUTOR NAME" "EXPECTED" "ACTUAL" "BALANCE"
    tuple 
    |> List.iter (fun (name, expected, actual, balance) -> 
        log.Info "| %-20s | %8.2f | %8.2f | %8.2f |" name expected actual balance)


[<EntryPoint>]
let main argv = 
    let log = CompositeLog(Constants.ApplciationLoggerName) :> ILog
    let dataTransferDirectory = new DirectoryInfo(Constants.DataTransferPath)
    let fileStore = FileStore(dataTransferDirectory, CompositeLog(Constants.FileStoreLoggerName))
    let dbStore = DbStore(dataTransferDirectory, CompositeLog(Constants.DbStoreLoggerName))
    
    DataLoader.loadPeople dbStore fileStore |> log.Info "%A"
    DataLoader.loadTransactions dbStore fileStore |> log.Info "%A"
    DataLoader.loadFunds dbStore fileStore |> log.Info "%A"

    FundProcessor.getBalancePerPerson dbStore 
    |> Result.map (printBalance log)
    |> ignore

    0
        