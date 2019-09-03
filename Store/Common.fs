[<RequireQualifiedAccess>]
module FundManager.Store.Common

    open FundManager.Logging

    let onError (log: ILog) (ex: exn) = 
        log.Error "Error message: %s" ex.Message
        log.Error "Stacktrace: \n%s" ex.StackTrace
        Error ex.Message