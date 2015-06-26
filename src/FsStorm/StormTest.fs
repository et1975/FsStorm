﻿module StormTest

open Storm
open FsJson

/// test runner for reliable spouts
let reliableSpoutRunner reliableEmit (cmds:Json seq) fCreateHousekeeper fCreateEmitter =
    async {
        try 
            let housekeeper = fCreateHousekeeper
            let emitter = fCreateEmitter 
            let next = emitter (reliableEmit housekeeper)
            for cmd in cmds do
                match cmd?command.Val with
                | NEXT            -> do! next()
                | ACK | FAIL | "" -> housekeeper cmd
                | _ -> failwithf "invalid cmd %A" cmd
        with ex ->
            return! stormLogAndThrow (nestedExceptionTrace ex) ()
    }

/// test runner loop for simple spouts
let simpleSpoutRunner emit (cmds:Json seq) fCreateEmitter =
    async {
        try 
            let emitter = fCreateEmitter emit
            for cmd in cmds do
                match cmd?command.Val with
                | NEXT            -> do! emitter()
                | ACK | FAIL | "" -> ()
                | _ -> failwithf "invalid cmd %A" cmd
        with ex ->
            return! stormLogAndThrow (nestedExceptionTrace ex) ()
    }

/// test bolt runner that auto acks received msgs
let autoAckBoltRunner testData stormAck fReaderCreator =
    async {
        try
            let reader = fReaderCreator 
            for jmsg in testData do
                match jmsg,jmsg?stream with
                | _, JsonString "__heartbeat" -> ()
                | x, _ when isArray x         -> () 
                | m, _                        -> 
                    do! reader jmsg 
                    match jmsg?id with
                    | JsonString str -> stormAck str
                    | _ -> ()
         with ex ->
            return! stormLogAndThrow (nestedExceptionTrace ex) ()
    }