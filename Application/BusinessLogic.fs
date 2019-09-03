namespace FundManager.Application

module BusinessLogic =

    open FundManager.Store
    open FundManager.Common.Result
    open FundManager.Domain

    let private toPersonEntity value =
        match value with 
        | Entity.Person x   -> [x]
        | _                 -> []

    let private toTransactionEntity value =
        match value with 
        | Entity.Transaction x  -> [x]
        | _                     -> []
    
    let private toFundEntity value =
        match value with
        | Entity.Fund x     -> [x]
        | _                 -> []


    [<RequireQualifiedAccess>]
    module DataLoader = 

        let loadFunds (dbStore: DbStore) (fileStore: FileStore) =
            result {    
                let! fileRawFundList = fileStore.GetFunds()
                let! dbRawFundList = dbStore.GetAll(EntityType.FundType)
                let dbFundList = dbRawFundList |> List.collect toFundEntity

                let fileFundList = 
                    fileRawFundList
                    |> List.sortBy (fun x -> ( x.YearMonth, x.Contributor ))
                    |> List.mapi (fun id fund -> {fund with Fund.Id = id + 1})

                do 
                    dbFundList
                    |> List.except fileFundList
                    |> List.map (fun x -> dbStore.Delete(x.Id |> EntityId.FundId))
                    |> ignore

                do 
                    fileFundList
                    |> List.except dbFundList
                    |> List.map Entity.Fund
                    |> List.map dbStore.Add
                    |> ignore

                return "Loaded funds from file and saved in DB"
            }

        let loadPeople (dbStore: DbStore) (fileStore: FileStore) = 
            result {
                let! fileRawPeopleList = fileStore.GetPeople()

                let! dbPeopleList = 
                    dbStore.GetAll(EntityType.PersonType)
                    |> Result.map (List.collect toPersonEntity)

                let filePeopleList = 
                    fileRawPeopleList 
                    |> List.sortBy (fun x -> (x.Name, x.Status))
                    |> List.mapi (fun id person -> {person with Person.Id = id + 1})

                do 
                    dbPeopleList
                    |> List.except filePeopleList
                    |> List.map (fun x -> dbStore.Delete (x.Id |> EntityId.PersonId))
                    |> ignore

                do 
                    filePeopleList
                    |> List.except dbPeopleList
                    |> List.map Entity.Person
                    |> List.map dbStore.Add
                    |> ignore

                return "Loaded people from file and saved in DB" 
            }
    
        let loadTransactions (dbStore: DbStore) (fileStore: FileStore) =
            result {    
                let! fileRawTransactionList = fileStore.GetTransactions()
                let! dbRawTransactionList = dbStore.GetAll(EntityType.TransactionType)
                let dbTransactionList = dbRawTransactionList |> List.collect toTransactionEntity

                let fileTransactionList = 
                    fileRawTransactionList
                    |> List.sortBy (fun x -> ( x.Date, x.Type, x.Description ))
                    |> List.mapi (fun id transaction -> {transaction with Transaction.Id = id + 1})

                do 
                    dbTransactionList
                    |> List.except fileTransactionList
                    |> List.map (fun x -> dbStore.Delete(x.Id |> EntityId.TransactionId))
                    |> ignore

                do 
                    fileTransactionList
                    |> List.except dbTransactionList
                    |> List.map Entity.Transaction
                    |> List.map dbStore.Add
                    |> ignore

                return "Loaded transactions from file and saved in DB"
            }


    [<RequireQualifiedAccess>]
    module FundProcessor = 
        let getBalancePerPerson (dbStore: DbStore) =
            result {
                let! transactions = 
                    dbStore.GetAll(EntityType.TransactionType)
                    |> Result.map (List.collect toTransactionEntity)

                let! funds = 
                    dbStore.GetAll(EntityType.FundType)
                    |> Result.map (List.collect toFundEntity)

                let expected =
                    funds
                    |> List.groupBy (fun x -> (x.Contributor))
                    |> List.map (fun (contributor, funds) -> 
                        (contributor, funds |> List.sumBy (fun x -> x.Amount)))

                let actual = 
                    transactions
                    |> List.filter (fun x -> x.Type = Transaction.Type.Deposit)
                    |> List.groupBy (fun x -> (x.Description))
                    |> List.map (fun (contributor, trans) -> 
                        (contributor, trans |> List.sumBy(fun x -> x.Amount)))
                
                let returnValue =
                    actual 
                    |> List.allPairs expected
                    |> List.filter (fun ((expected, _), (actual, _)) -> expected = actual)
                    |> List.map (fun ((contributor, expectedAmount), (_, actualAmount)) -> 
                        (contributor, expectedAmount, actualAmount, expectedAmount - actualAmount))
                    |> List.filter (fun (_, _, _, balance) -> balance <> 0M)
                    |> List.sortBy (fun (name, _, _, _) -> name)

                return returnValue
            }