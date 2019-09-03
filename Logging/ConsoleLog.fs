namespace FundManager.Logging

type ConsoleLog() =
    interface ILog with 
        member __.Fatal fmt   = Printf.kprintf System.Console.WriteLine fmt
        member __.Error fmt   = Printf.kprintf System.Console.WriteLine fmt
        member __.Warn fmt    = Printf.kprintf System.Console.WriteLine fmt
        member __.Debug fmt   = Printf.kprintf System.Console.WriteLine fmt
        member __.Info fmt    = Printf.kprintf System.Console.WriteLine fmt
        member __.Trace fmt   = Printf.kprintf System.Console.WriteLine fmt
