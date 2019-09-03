namespace FundManager.Logging

type CompositeLog (name: string) = 
    let log = NLog.LogManager.GetLogger(name)
    interface ILog with
        member __.Fatal fmt   = Printf.kprintf log.Fatal fmt
        member __.Error fmt   = Printf.kprintf log.Error fmt
        member __.Warn fmt    = Printf.kprintf log.Warn fmt
        member __.Debug fmt   = Printf.kprintf log.Debug fmt
        member __.Info fmt    = Printf.kprintf log.Info fmt
        member __.Trace fmt   = Printf.kprintf log.Trace fmt

        