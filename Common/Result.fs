module FundManager.Common.Result

    type ResultBuilder() =
        member __.Return(x) = Ok x
        member __.Bind(x,f) = Result.bind f x
        member __.Zero() = None

    let result = new ResultBuilder()

    let ofOption error value = 
        match value with
        | Some x -> Ok x 
        | None -> Error error