open System
open System.IO

// File path for reference
// @C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug

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


let HandleInput(input: string) =
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


let CheckInput(input: string) =
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


let TestStructFunc() = 
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


let MainLoop() = 
    let mutable quit = false
    while quit <> true do
        let input = Console.ReadLine()

        match input with
        | "q" -> quit <- true
        | _ -> CheckInput(input)


type Data() = 
    member x.Read() =
        use stream = new StreamReader @"C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug\StoreCodes.csv"

        let testMap =
            let mutable m = Map.empty
            let mutable valid = true
            while (valid) do
                let line = stream.ReadLine()
                if (line = null) then
                    valid <- false
                else
                    let foo = line.Split ','
                    printfn "%s %s" foo.[0] foo.[1]


        printfn "%A" testMap
        (*let storeListMap = Map.empty

        let mutable valid = true
        let mutable bar = 0
        while (valid) do
            let line = stream.ReadLine()
            if (line = null) then
                valid <- false
            else
                let foo = line.Split ','
                Add(foo.[0], foo.[1])*)
            


let students =
    Map.empty. (* Creating an empty Map *)
        Add("Zara Ali", "1501").
        Add("Rishita Gupta", "1502").
        Add("Robin Sahoo", "1503").
        Add("Gillian Megan", "1504");;
printfn "Map - students: %A" students

[<EntryPoint>]
let main argv = 
    //let data = new Data()
    //data.Read()



    //MainLoop()
    printfn "Goodbye"
    //Console.ReadKey()
    0 // return an integer exit code
