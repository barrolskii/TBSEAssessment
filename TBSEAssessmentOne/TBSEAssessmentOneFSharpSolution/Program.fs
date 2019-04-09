// Learn more about F# at http://fsharp.org

open System

let PrintCommands() =
    printfn "===================================================================================\n"
    printfn "-h or -help for a list of all commands"
    printfn "-ps or -printstores to print all stores"
    printfn "-pst or -printsuppliertypes to print all supplier types"
    printfn "-psup or -printsuppliers to print all suppliers"
    printfn "\n==================================================================================="


let PrintSelectionOptions() =
    printfn "==================================================================================="
    printfn "1: Total cost of all orders"
    printfn "2: Cost of all orders for a single store"
    printfn "3: Cost of all orders in a week for all stores"
    printfn "4: Cost of all orders in a week for a single store"
    printfn "5: Cost of all orders to a supplier"
    printfn "6: Cost of all orders to a supplier type"
    printfn "7: Cost of orders in a week for a supplier type"
    printfn "8: Cost of orders to a supplier type for a store"
    printfn "9: Cost of orders in a week for a supplier type for a store"
    printfn "==================================================================================="


let PrintStores() =
    printfn "Print stores"


let PrintSupplierTypes() =
    printfn "Print supplier types"


let PrintSuppliers() =
    printfn "Print suppliers"


let foo() =
    let bar = Console.ReadLine()
    match bar with
    | "-h" -> PrintCommands()
    | "-help" -> PrintCommands()
    | "-ps" -> PrintStores()
    | "-printstores" -> PrintStores()
    | "-pst" -> PrintSupplierTypes()
    | "-printsuppliertypes" -> PrintSupplierTypes()
    | "-psup" -> PrintSuppliers()
    | "-printsuppliers" -> PrintSuppliers()
    | _ -> printfn "Boi"


[<EntryPoint>]
let main argv =
    foo()

    printfn "Hello World from F#!"
    0 // return an integer exit code
