namespace FundManager.Logging

open NLog
open Microsoft.FSharp.Core

type ILog = 
    abstract Fatal  : Printf.StringFormat<'a, unit> -> 'a
    abstract Error  : Printf.StringFormat<'a, unit> -> 'a
    abstract Warn   : Printf.StringFormat<'a, unit> -> 'a
    abstract Debug  : Printf.StringFormat<'a, unit> -> 'a
    abstract Info   : Printf.StringFormat<'a, unit> -> 'a
    abstract Trace  : Printf.StringFormat<'a, unit> -> 'a



        