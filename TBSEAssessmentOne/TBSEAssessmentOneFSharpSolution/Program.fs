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


let TotalCostOfAllOrders() =
    printfn "Called"


let CostOfAllOrdersForASingleStore() =
    printfn "Called"


let CostOfAllOrdersInAWeekForAllStores() =
    printfn "Called"


let CostOfAllOrdersInASingleWeekForAStore() =
    printfn "Called"


let CostOfAllOrdersToASupplier() =
    printfn "Called"


let CostOfAllOrdersToASupplierType() =
    printfn "Called"


let CostOfAllOrdersInAWeekToASupplierType() =
    printfn "Called"


let CostOfAllOrdersToASupplierTypeForAStore() =
    printfn "Called"


let CostOfAllOrdersInAWeekToASupplierTypeForAStore() =
    printfn "Called"


let HandleInput(input) =
    match input with
    | "1" -> TotalCostOfAllOrders()
    | "2" -> CostOfAllOrdersForASingleStore()
    | "3" -> CostOfAllOrdersInAWeekForAllStores()
    | "4" -> CostOfAllOrdersInASingleWeekForAStore()
    | "5" -> CostOfAllOrdersToASupplier()
    | "6" -> CostOfAllOrdersToASupplierType()
    | "7" -> CostOfAllOrdersInAWeekToASupplierType()
    | "8" -> CostOfAllOrdersToASupplierTypeForAStore()
    | "9" -> CostOfAllOrdersInAWeekToASupplierTypeForAStore()
    | _ -> printfn "Please select a valid option or enter q to quit"


let CheckCommands() =
    let input = Console.ReadLine()
    match input with
    | "-h" -> PrintCommands()
    | "-help" -> PrintCommands()
    | "-ps" -> PrintStores()
    | "-printstores" -> PrintStores()
    | "-pst" -> PrintSupplierTypes()
    | "-printsuppliertypes" -> PrintSupplierTypes()
    | "-psup" -> PrintSuppliers()
    | "-printsuppliers" -> PrintSuppliers()
    | _ -> HandleInput(input)


type Date = 
    struct
        val week: int
        val year: int

        new (initWeek, initYear) =
            { week = initWeek; year = initYear }
    end


type Store =
    struct
        val storeCode: string
        val storeLoc: string

        new (code, loc) =
            { storeCode = code; storeLoc = loc }
    end


type Order =
    struct
        val store: Store
        val date: Date
        val supplier: string
        val supplierType: string
        val cost: double

        new (s, d, sup, supt, c) =
            { store = s; date = d; supplier = sup; supplierType = supt; cost = c; }
    end


[<EntryPoint>]
let main argv = 
    let test = new Date(2, 2014)
    printfn "week %d" test.week
    printfn "year %d" test.year

    let test2 = new Store("NEW1", "Newport")
    printfn "\ncode %s" test2.storeCode
    printfn "loc %s" test2.storeLoc

    let test3 = new Order(test2, test, "Heinz", "Food", 2.80)
    printfn "\ncode: %s" test3.store.storeCode
    printfn "loc: %s" test3.store.storeLoc
    printfn "week: %d" test3.date.week
    printfn "year: %d" test3.date.year
    printfn "supplier: %s" test3.supplier
    printfn "type: %s" test3.supplierType
    printfn "cost: %g" test3.cost

    CheckCommands()
    Console.ReadKey()
    0 // return an integer exit code
