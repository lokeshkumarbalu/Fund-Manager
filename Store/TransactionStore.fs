namespace FundManager.Store

open System
open System.IO
open FundManager.Domain
open FundManager.Logging
open FundManager.Store

type ITransactionStore =
   abstract member Add: Transaction -> Result<TransactionId, string>
   abstract member Delete: Transaction -> Result<bool, string>
   abstract member Find: TransactionId -> Result<Transaction, string>
   abstract member GetAll: unit -> Result<Transaction list, string>
   abstract member Update: Transaction -> Result<bool, string>

type TransactionStore (dbDirectory: DirectoryInfo, log: ILog) =
    let onError ex = Common.onError log ex
    let db = Common.getConfiguredLiteDB dbDirectory

    interface ITransactionStore with
        member __.Add(transaction: Transaction): Result<TransactionId, string> =
            db.GetCollection<Transaction>(Constants.CollectionNames.Transaction)
            |> Common.add log transaction
            |> Result.bind (fun x -> x |> int |> Ok)

        member __.Delete(transaction: Transaction): Result<bool, string> = 
            db.GetCollection<Transaction>(Constants.CollectionNames.Transaction)
            |> Common.delete log transaction.Id

        member __.Find(transactionId: TransactionId): Result<Transaction, string> = 
            db.GetCollection<Transaction>(Constants.CollectionNames.Transaction)
            |> Common.find log transactionId

        member __.GetAll(): Result<Transaction list, string> = 
            db.GetCollection<Transaction>(Constants.CollectionNames.Transaction)
            |> Common.findAll log 

        member __.Update(transaction: Transaction): Result<bool, string> = 
            db.GetCollection<Transaction>(Constants.CollectionNames.Transaction) 
            |> Common.update log transaction

    interface IDisposable with 
        member __.Dispose() = 
            db.Dispose()

