﻿// ========================================================================= //
// Copyright (c) 2003-2007, John Harrison.                                   //
// Copyright (c) 2012 Eric Taucher, Jack Pappas, Anh-Dung Phan               //
// (See "LICENSE.txt" for details.)                                          //
// ========================================================================= //

module Reasoning.Automated.Harrison.Handbook.Tests.prop

open Reasoning.Automated.Harrison.Handbook.lib
open Reasoning.Automated.Harrison.Handbook.intro
open Reasoning.Automated.Harrison.Handbook.formulas
open Reasoning.Automated.Harrison.Handbook.prop
open NUnit.Framework
open FsUnit

// pg. 33
// ------------------------------------------------------------------------- //
// Example of use.                                                           //
// ------------------------------------------------------------------------- //

[<TestCase(true, false, true, Result = true)>]
[<TestCase(true, true, false, Result = false)>]
let ``eval``(p, q, r) =
    function
        | P "p" -> p
        | P "q" -> q
        | P "r" -> r
        | _ -> failwith "Invalid property name"
    |> eval (parse_prop_formula "p /\ q ==> q /\ r")

// pg. 35
// ------------------------------------------------------------------------- //
// Example.                                                                  //
// ------------------------------------------------------------------------- //

[<Test>]
let ``atoms``() =
    atoms (parse_prop_formula "p /\ q \/ s ==> ~p \/ (r <=> s)")
    |> should equal [P "p"; P "q"; P "r"; P "s"]

// pg. 41
// ------------------------------------------------------------------------- //
// Examples.                                                                 //
// ------------------------------------------------------------------------- //

[<TestCase(@"p \/ ~p", Result = true)>]
[<TestCase(@"p \/ q ==> p", Result = false)>]
[<TestCase(@"p \/ q ==> q \/ (p <=> q)", Result = false)>]
[<TestCase(@"(p \/ q) /\ ~(p /\ q) ==> (~p <=> q)", Result = true)>]
let ``tautology all`` formula  =
    tautology (parse_prop_formula formula)

// pg. 43
// ------------------------------------------------------------------------- //
// Surprising tautologies including Dijkstra's "Golden rule".                //
// ------------------------------------------------------------------------- //

[<TestCase(@"(p ==> q) \/ (q ==> p)", Result = true)>]
[<TestCase(@"p \/ (q <=> r) <=> (p \/ q <=> p \/ r)", Result = true)>]
[<TestCase(@"p /\ q <=> ((p <=> q) <=> p \/ q)", Result = true)>]
[<TestCase(@"(p ==> q) <=> (~q ==> ~p)", Result = true)>]
[<TestCase(@"(p ==> ~q) <=> (q ==> ~p)", Result = true)>]
[<TestCase(@"(p ==> q) <=> (q ==> p)", Result = false)>]
let ``surprising tautology`` formula =
    tautology (parse_prop_formula formula)

// pg. 47
// ------------------------------------------------------------------------- //
// Some logical equivalences allowing elimination of connectives.            //
// ------------------------------------------------------------------------- //

[<Test>]
let ``equivalences``() =
    List.forall tautology [
        parse_prop_formula "true <=> false ==> false";
        parse_prop_formula "~p <=> p ==> false";
        parse_prop_formula "p /\ q <=> (p ==> q ==> false) ==> false";
        parse_prop_formula "p \/ q <=> (p ==> false) ==> q";
        parse_prop_formula "(p <=> q) <=> ((p ==> q) ==> (q ==> p) ==> false) ==> false"; ]
    |> should be True

// pg. 53
// ------------------------------------------------------------------------- //
// Example of NNF function in action.                                        //
// ------------------------------------------------------------------------- //

[<Test>]
let ``nnf``() =
    let fm003 = (parse_prop_formula ("(p <=> q) <=> ~(r ==> s)"))
    let fm003' = nnf fm003
    tautology(Iff(fm003,fm003'))
    |> should be True

// pg. 54
// ------------------------------------------------------------------------- //
// Some tautologies remarked on.                                             //
// ------------------------------------------------------------------------- //

[<TestCase(@"(p ==> p') /\ (q ==> q') ==> (p /\ q ==> p' /\ q')", Result = true)>]
[<TestCase(@"(p ==> p') /\ (q ==> q') ==> (p \/ q ==> p' \/ q')", Result = true)>]
let ``remarked tautology`` formula  =
    tautology (parse_prop_formula formula)

// pg. 58
// ------------------------------------------------------------------------- //
// Example.                                                                  //
// ------------------------------------------------------------------------- //

[<Test>]
let ``purednf all``() =
    purednf (parse_prop_formula ("(p \/ q /\ r) /\ (~p \/ ~r)"))
    |> should equal [[Atom (P "p"); Not (Atom (P "p"))]; 
                        [Atom (P "p"); Not (Atom (P "r"))]; 
                        [Atom (P "q"); Atom (P "r"); Not (Atom (P "p"))]; 
                        [Atom (P "q"); Atom (P "r"); Not (Atom (P "r"))]]

// pg. 59
// ------------------------------------------------------------------------- //
// Example.                                                                  //
// ------------------------------------------------------------------------- //

[<Test>]
let ``non-trivial purednf``() =
    List.filter (non trivial) (purednf (parse_prop_formula ("(p \/ q /\ r) /\ (~p \/ ~r)")))
    |> should equal [[Atom (P "p"); Not (Atom (P "r"))]; 
                        [Atom (P "q"); Atom (P "r"); Not (Atom (P "p"))]]

// pg. 59
// ------------------------------------------------------------------------- //
// Example.                                                                  //
// ------------------------------------------------------------------------- //
    
[<Test>]
let ``dnf``() =
    let fm005 = (parse_prop_formula ("(p \/ q /\ r) /\ (~p \/ ~r)"))
    tautology(Iff(fm005,dnf fm005))
    |> should be True

// pg. 61
// ------------------------------------------------------------------------- //
// Example.                                                                  //
// ------------------------------------------------------------------------- //
    
[<Test>]
let ``cnf``() =
    let fm006 = (parse_prop_formula ("(p \/ q /\ r) /\ (~p \/ ~r)"))
    tautology(Iff(fm006,cnf fm006))
    |> should be True