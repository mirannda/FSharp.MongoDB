namespace FSharp.MongoDB.Bson.Serialization

open Microsoft.FSharp.Reflection

open MongoDB.Bson.Serialization

open FSharp.MongoDB.Bson.Serialization.Serializers

type FSharpValueSerializationProvider() =

    let isUnion typ = FSharpType.IsUnion typ

    let isOption typ = isUnion typ && typ.IsGenericType
                                   && typ.GetGenericTypeDefinition() = typedefof<_ option>

    interface IBsonSerializationProvider with
        member __.GetSerializer(typ : System.Type) =

            // Check that `typ` is an option type
            if isOption typ then
                OptionTypeSerializer(typ) :> IBsonSerializer

            // Check that `typ` is the overall union type, and not a particular union case
            elif isUnion typ && typ.BaseType = typeof<obj> then
                let nested = typ.GetNestedTypes() |> Array.filter isUnion
                let props = typ.GetProperties() |> Array.filter (fun x -> isUnion x.PropertyType)

                // Handles non-singleton discriminated unions
                if nested.Length > 0 || props.Length > 0 then
                    nested |> Array.iter (fun x -> BsonClassMap.LookupClassMap x |> ignore)
                    DiscriminatedUnionSerializer(typ) :> IBsonSerializer

                // Handles singleton discriminated unions
                else
                    let classMap = BsonClassMap.LookupClassMap typ
                    BsonClassMapSerializer(classMap) :> IBsonSerializer

            // Otherwise, signal we do not provide serialization for this type
            else null

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Serializers =

    let mutable private registered = false

    [<CompiledName("Register")>]
    let register() =
        if not registered then
            registered <- true
            BsonSerializer.RegisterSerializationProvider(FSharpValueSerializationProvider())