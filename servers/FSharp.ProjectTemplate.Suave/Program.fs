﻿namespace FSharp.ProjectTemplate.Suave

module Program =

    open System

    open Suave
    open Suave.Web
    open Suave.Http
    open Suave.Http.Successful
    open Suave.Http.RequestErrors
    open Suave.Http.Applicatives
    open Suave.Types
    open Suave.Utils

    open Orleankka
    open Orleankka.Http
    open Orleankka.Playground

    open System.Reflection
    open Newtonsoft.Json

    open FSharp.ProjectTemplate.Actors

    open Serilog
    open Orleans.Serialization

    open JsonSerializer

    open FSharp.ProjectTemplate.Domain

    open Support

    let db = DI.Load<FSharp.ProjectTemplate.IHelloPersistency> ()
    //let db = DI.Register<FSharp.ProjectTemplate.NMemory.Impl.Database, FSharp.ProjectTemplate.IHelloPersistency> ()

    type GreeterSQL() = 
        inherit Greeter()

        override x.SaveLastHello (p : Person) = 
            db.Save(p)
        override x.LoadLastHello (p : Person) = 
            db.Load(p)

    [<EntryPoint>]
    let main argv = 
      Log.Logger <- LoggerConfiguration()
            .Destructure.FSharpTypes()
            .WriteTo.Console()
            .CreateLogger()
      Log.Information( "Web server started" )

      let assemblies:Assembly [] = [| Assembly.GetExecutingAssembly();(typeof<Greeter>).Assembly |]

      // configure actor system
      use system = ActorSystem.Configure()
                              .Playground()
                              .Register(assemblies)
                              .Done()
  
      let testActor = system.ActorOf<GreeterSQL>("http_test")

      Log.Debug( "Actor path {@ActorPath}", testActor.Path )
      printfn "%A" testActor.Path

      // configure actor routing
      let router = [(MessageType.DU(typeof<HelloMessage>), testActor.Path)]
                    |> Seq.collect HttpRoute.create
                    |> ActorRouter.create JsonConvert.DeserializeObject


      let hasContentType (ctx:HttpContext) = async {
        match ctx.request.header "content-type" with
        | Choice1Of2 v when v = ContentType.Orleankka -> 
               return Some ctx
        | _ -> return None
      }    

      // sends msg to actor 
      let sendMsg actorPath (ctx:HttpContext) = async {    
    
        let msgBody = ctx.request.rawForm |> UTF8.toString
        
        match router.Dispatch(actorPath, msgBody) with
        | Some t -> let! result = Async.AwaitTask t
                    return! OK (result.ToString()) ctx
        | None   -> return! BAD_REQUEST "actor was not found, or message has invalid format" ctx  
      }  

      // configure Suave routing
      let app = POST >>= hasContentType >>= pathScan "/api/%s" (fun path -> request (fun req ctx -> sendMsg path ctx))  

      printfn "Finished booting cluster...\n"

      startWebServer defaultConfig app
      0 
