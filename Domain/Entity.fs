namespace FundManager.Domain

type Entity =
    | Fund          of Fund.Entity
    | Person        of Person.Entity
    | Transaction   of Transaction.Entity

type EntityId = 
    | FundId        of Fund.Id
    | PersonId      of Person.Id
    | TransactionId of Transaction.Id

type EntityType =
    | FundType
    | PersonType
    | TransactionType

