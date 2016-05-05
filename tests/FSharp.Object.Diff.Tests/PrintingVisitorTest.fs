﻿module FSharp.Object.Diff.Tests.PrintingVisitorTest

open System
open System.Text
open Persimmon
open UseTestNameByReflection
open FSharp.Object.Diff

type TestablePrintingVisitor(working: obj, base_: obj) =
  inherit PrintingVisitor(working, base_)
  let sb = StringBuilder()
  override __.Print(text) = sb.AppendLine(text) |> ignore
  member __.Output = sb.ToString()

let ``omits intermediate nodes with changed child nodes`` = skip "" <| test {
  let c1 = { Id = "c"; Reference = None }
  let b1 = { Id = "b"; Reference = Some c1 }
  let a1 = { Id = "a"; Reference = Some b1 }
  let d2 = { Id = "d"; Reference = None }
  let b2 = { Id = "b"; Reference = Some d2 }
  let a2 = { Id = "a"; Reference = Some b2 }
  let rootNode = ObjectDifferBuilder.BuildDefault().Compare(a1, a2)
  let visitor = TestablePrintingVisitor(a1, a2)
  rootNode.Visit(visitor)
  do! assertEquals ("Property at path '/reference/reference/id' has changed from [ d ] to [ c ]" + Environment.NewLine) visitor.Output 
}

let ``prints root node if unchanged and without children`` = test {
  let visitor = TestablePrintingVisitor("foo", "foo")
  let rootNode = DiffNode.NewRootNodeWithType(typeof<string>)
  rootNode.Visit(visitor)
  do! assertEquals ("Property at path '/' has not changed" + Environment.NewLine) visitor.Output 
}
