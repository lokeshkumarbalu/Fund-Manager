namespace FundManager.Application

open FundManager.Store

module BusinessLogic =

    open FundManager.Common.Result
    open FundManager.Domain

    [<RequireQualifiedAccess>]
    module DataLoader = 

        let loadFunds (fundStore: IFundStore) (fileStore: FileStore) =
            result {    
                let! fileRawFundList = fileStore.GetFunds()
                let! dbFundList = fundStore.GetAll()

                let fileFundList = 
                    fileRawFundList
                    |> List.sortBy (fun x -> ( x.YearMonth, x.Contributor ))
                    |> List.mapi (fun id fund -> {fund with Fund.Id = id + 1})

                do 
                    dbFundList
                    |> List.except fileFundList
                    |> List.map fundStore.Delete
                    |> ignore

                do 
                    fileFundList
                    |> List.except dbFundList
                    |> List.map fundStore.Add
                    |> ignore

                return "Loaded funds from file and saved in DB"
            }

        let loadPeople (personStore: IPersonStore) (fileStore: FileStore) = 
            result {
                let! fileRawPeopleList = fileStore.GetPeople()
                let! dbPeopleList = personStore.GetAll()

                let filePeopleList = 
                    fileRawPeopleList 
                    |> List.sortBy (fun x -> (x.Name, x.Status))
                    |> List.mapi (fun id person -> {person with Person.Id = id + 1})

                do 
                    dbPeopleList
                    |> List.except filePeopleList
                    |> List.map personStore.Delete
                    |> ignore

                do 
                    filePeopleList
                    |> List.except dbPeopleList
                    |> List.map personStore.Add
                    |> ignore

                return "Loaded people from file and saved in DB" 
            }
    
        let loadTransactions (transactionStore: ITransactionStore) (fileStore: FileStore) =
            result {    
                let! fileRawTransactionList = fileStore.GetTransactions()
                let! dbTransactionList = transactionStore.GetAll()

                let fileTransactionList = 
                    fileRawTransactionList
                    |> List.sortBy (fun x -> ( x.Date, x.Type, x.Description ))
                    |> List.mapi (fun id transaction -> {transaction with Transaction.Id = id + 1})

                do 
                    dbTransactionList
                    |> List.except fileTransactionList
                    |> List.map transactionStore.Delete
                    |> ignore

                do 
                    fileTransactionList
                    |> List.except dbTransactionList
                    |> List.map transactionStore.Add
                    |> ignore

                return "Loaded transactions from file and saved in DB"
            }


    [<RequireQualifiedAccess>]
    module FundProcessor = 
        open FundManager.Logging

        let getBalance (transactionStore: ITransactionStore) (fundStore: IFundStore) =
            result {
                let! transactions = transactionStore.GetAll()
                let! funds = fundStore.GetAll()

                let expected =
                    funds
                    |> List.groupBy (fun x -> (x.Contributor))
                    |> List.map (fun (contributor, funds) -> 
                        (contributor, funds |> List.sumBy (fun x -> x.Amount)))

                let actual = 
                    transactions
                    |> List.filter (fun x -> x.Type = TransactionAttributes.Type.Deposit)
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
        
        let printBalance (log: ILog) tuple =
            log.Info "| %-20s | %8s | %8s | %8s |" "CONTRIBUTOR NAME" "EXPECTED" "ACTUAL" "BALANCE"
            tuple 
            |> List.iter (fun (name, expected, actual, balance) -> 
            log.Info "| %-20s | %8.2M | %8.2M | %8.2M |" name expected actual balance)