﻿namespace FSharp.ProjectTemplate.DbTests.Autofac

module Tests =

    open FSharp.ProjectTemplate
    open NUnit.Framework
    open FSharp.ProjectTemplate.Domain
    open Serilog
    open System
    open Support

    Log.Logger <- LoggerConfiguration()
        .Destructure.FSharpTypes()
        .WriteTo.Console()
        .CreateLogger()
    Log.Information( "Tests started" )


    [<Test>]
    let HokusPokus () =
        let a = FSharp.ProjectTemplate.NMemory.Impl.Database()
        Assert.IsNotNull(a)

    [<Test>]
    let ``simple database crud is working`` () =
      //let db = DI.Load<IHelloPersistency> ()
      let db = DI.Register<FSharp.ProjectTemplate.NMemory.Impl.Database, IHelloPersistency> ()
      let p = {FirstName="John";LastName="Rambo"}
      db.Save( p )
      let lastSeen = db.Load( p )
      Assert.AreEqual( true, lastSeen.IsSome )
      Assert.LessOrEqual( DateTime.Now - lastSeen.Value, TimeSpan.FromSeconds(float 1) )
