(* Copyright (c) 2013 MongoDB, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *)

namespace FSharp.MongoDB.Bson.Tests.Serialization

open MongoDB.Bson

open NUnit.Framework
open Swensen.Unquote

module FSharpSetSerialization =

    type Record = { Ints : Set<int> }

    [<Test>]
    let ``test serialize an empty set``() =
        let value = { Ints = Set.empty<int> }

        let result = <@ serialize value @>
        let expected = BsonDocument("Ints", BsonArray Set.empty<int>)

        test <@ %result = expected @>

    [<Test>]
    let ``test deserialize an empty set``() =
        let doc = BsonDocument("Ints", BsonArray Set.empty<int>)

        let result = <@ deserialize doc typeof<Record> @>
        let expected = { Ints = Set.empty<int> }

        test <@ %result = expected @>

    [<Test>]
    let ``test serialize a set of one element``() =
        let value = { Ints = Set.ofList [ 0 ] }

        let result = <@ serialize value @>
        let expected = BsonDocument("Ints", BsonArray [ 0 ])

        test <@ %result = expected @>

    [<Test>]
    let ``test deserialize a set of one element``() =
        let doc = BsonDocument("Ints", BsonArray [ 0 ])

        let result = <@ deserialize doc typeof<Record> @>
        let expected = { Ints = Set.ofList [ 0 ] }

        test <@ %result = expected @>

    [<Test>]
    let ``test serialize a set of multiple elements``() =
        let value = { Ints = Set.ofList [ 1; 2; 3 ] }

        let result = <@ serialize value @>
        let expected = BsonDocument("Ints", BsonArray (Set.ofList [ 1; 2; 3 ]))

        test <@ %result = expected @>

    [<Test>]
    let ``test deserialize a set of multiple elements``() =
        let doc = BsonDocument("Ints", BsonArray [ 1; 2; 3 ])

        let result = <@ deserialize doc typeof<Record> @>
        let expected = { Ints = Set.ofList [ 1; 2; 3 ] }

        test <@ %result = expected @>

    module OptionType =

        type Record = { MaybeStrings : Set<string option> }

        [<Test>]
        let ``test serialize a set of optional strings``() =
            let value = { MaybeStrings = Set.ofList [ Some "a"; None; Some "z" ] }

            let result = <@ serialize value @>
            let expected = BsonDocument("MaybeStrings", BsonArray (Set.ofList ["a"; null; "z"]))

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a set of optional strings``() =
            let doc = BsonDocument("MaybeStrings", BsonArray [ "a"; null; "z" ])

            let result = <@ deserialize doc typeof<Record> @>
            let expected = { MaybeStrings = Set.ofList [ Some "a"; None; Some "z" ] }

            test <@ %result = expected @>

    module RecordType =

        type KeyValuePair =
            { Key : string
              Value : int }

        type Record = { Elements : Set<KeyValuePair> }

        [<Test>]
        let ``test serialize a set of record types``() =
            let value = { Elements = Set.ofList [ { Key = "a"; Value = 1 }
                                                  { Key = "b"; Value = 2 }
                                                  { Key = "c"; Value = 3 } ] }

            let result = <@ serialize value @>
            let expected =
                BsonDocument(
                    "Elements",
                    BsonArray [ BsonDocument [ BsonElement("Key", BsonString "a")
                                               BsonElement("Value", BsonInt32 1) ]
                                BsonDocument [ BsonElement("Key", BsonString "b")
                                               BsonElement("Value", BsonInt32 2) ]
                                BsonDocument [ BsonElement("Key", BsonString "c")
                                               BsonElement("Value", BsonInt32 3) ] ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a set of record types``() =
            let doc =
                BsonDocument(
                    "Elements",
                    BsonArray [ BsonDocument [ BsonElement("Key", BsonString "a")
                                               BsonElement("Value", BsonInt32 1) ]
                                BsonDocument [ BsonElement("Key", BsonString "b")
                                               BsonElement("Value", BsonInt32 2) ]
                                BsonDocument [ BsonElement("Key", BsonString "c")
                                               BsonElement("Value", BsonInt32 3) ] ])

            let result = <@ deserialize doc typeof<Record> @>
            let expected = { Elements = Set.ofList [ { Key = "a"; Value = 1 }
                                                     { Key = "b"; Value = 2 }
                                                     { Key = "c"; Value = 3 } ] }

            test <@ %result = expected @>
