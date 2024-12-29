using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Dealer
{
    // Name is unique
    public string                   name;
    public List<Cars.CarClass>      carClasses;
    public List<Cars.CarBrand>      carBrands;
    public List<Purchasable>        products;

    public Dealer(string nameParam, List<Cars.CarClass> carClassesParam, List<Cars.CarBrand> carBrandsParam){
        name            = nameParam;
        carClasses      = carClassesParam;
        carBrands       = carBrandsParam;
        products        = new List<Purchasable>();
    }

    public Sprite GetSprite(){
        string imageName = name.Replace(" ", "") + "_Dealer";
        return Resources.Load<Sprite>("Images/" + imageName);
    }

    public void AddProduct(Purchasable product){
        if(!products.Contains(product)){
            products.Add(product);
        }
    }
}

public class CarDealer : Dealer
{
    public CarDealer(string nameParam, List<Cars.CarClass> carClassesParam, List<Cars.CarBrand> carBrandsParam) : base(nameParam, carClassesParam, carBrandsParam) {

        // Add in all the cars that match the classes/brands of this dealer
        foreach(Cars.CarClass carClass in carClasses){
            // For each class this dealer sells, add cars that are this class
            List<Car> carsToAdd  = carClass == Cars.CarClass.None ? new List<Car>() : Cars.GetCarsWithClass(carClass);
            foreach(Car car in carsToAdd){
                AddProduct(car);
            }
        }

        foreach(Cars.CarBrand carBrand in carBrands){
            // For each brand this dealer sells, add cars that are this brand
            List<Car> carsToAdd  = carBrand == Cars.CarBrand.None ? new List<Car>() : Cars.GetCarsWithBrand(carBrand);
            foreach(Car car in carsToAdd){
                AddProduct(car);
            }
        }
    }
}

public class EntryPassDealer : Dealer
{
    public EntryPassDealer(string nameParam,  List<Cars.CarClass> carClassesParam, List<Cars.CarBrand> carBrandsParam) : base(nameParam, carClassesParam, carBrandsParam) {
        // Add in all the entry passes that match the classes/brands of this dealer
        foreach(Cars.CarClass carClass in carClasses){
            // For each class this dealer sells, add entry pass that are this class
            List<EntryPass> passesToAdd  = carClass == Cars.CarClass.None ? new List<EntryPass>() : EntryPasses.GetPassesWithClass(carClass);
            foreach(EntryPass entryPass in passesToAdd){
                AddProduct(entryPass);
            }
        }

        foreach(Cars.CarBrand carBrand in carBrands){
            // For each brand this dealer sells, add cars that are this brand
            List<EntryPass> passesToAdd  = carBrand == Cars.CarBrand.None ? new List<EntryPass>() : EntryPasses.GetPassesWithBrand(carBrand);
            foreach(EntryPass entryPass in passesToAdd){
                AddProduct(entryPass);
            }
        }
    }
}

public static class Dealers
{
    // DEVS: change the contents of this list to add new types of dealers (whos data is generated by the below array)
    public static List<Type>        dealerTypes   = new List<Type>() { typeof(CarDealer), typeof(EntryPassDealer) } ;
    // DEVS: change the contents of this array to add new dealers to the initial DB
    private static List<object[]>   paramList     = new List<object[]>()
        {
            new object[]
            { VEE_NAME,               new List<Cars.CarClass> {Cars.CarClass.FormulaVeeBrasil},   new List<Cars.CarBrand> {Cars.CarBrand.None} },
            new object[]
            { COPA_CLASSIC_B_NAME,    new List<Cars.CarClass> {Cars.CarClass.CopaClassicB},       new List<Cars.CarBrand> {Cars.CarBrand.None} },
            new object[]
            { FORD_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Ford} },
            new object[]
            { VOLKSWAGEN_NAME,        new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Volkswagen} },
            new object[]
            { CHEVROLET_NAME,         new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Chevrolet} },
            new object[]
            { PUMA_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Puma} },
            new object[]
            { FIAT_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Fiat} },
            new object[]
            { MINI_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Mini} }
        };

    public const string                                             VEE_NAME                = "Formula Vee";
    public const string                                             COPA_CLASSIC_B_NAME     = "Copa Classic - B";
    public const string                                             FORD_NAME               = "Ford";
    public const string                                             VOLKSWAGEN_NAME         = "Volkswagen";
    public const string                                             CHEVROLET_NAME          = "Chevrolet";
    public const string                                             PUMA_NAME               = "Puma";
    public const string                                             FIAT_NAME               = "Fiat";
    public const string                                             MINI_NAME               = "Mini";

   // Holds ALL dealers in the database
    private const string                                            ALL_DEALERS_KEY         = "dealers";
    // Will match unique name to dealer
    private const string                                            NAME_TO_DEALER_KEY      = "nameToDealer";
    // Will match class to list of dealers who sell that class
    private const string                                            CLASS_TO_DEALERS_KEY    = "classToDealers";
    // Will match brand to list of dealers who sell that brand
    private const string                                            BRAND_TO_DEALERS_KEY    = "brandToDealers";

    private static Dictionary<Type, Dictionary<string, dynamic>>    dealerDict {get; set;}

    static Dealers(){

        dealerDict                                          = new Dictionary<Type, Dictionary<string, dynamic>>();

        foreach(Type dealerType in dealerTypes){
            dealerDict[dealerType]                          = new Dictionary<string, dynamic>();
            dealerDict[dealerType][ALL_DEALERS_KEY]         = new List<Dealer>();
            dealerDict[dealerType][NAME_TO_DEALER_KEY]      = new Dictionary<string, Dealer>();
            dealerDict[dealerType][CLASS_TO_DEALERS_KEY]    = new Dictionary<Cars.CarClass, List<Dealer>>();
            dealerDict[dealerType][BRAND_TO_DEALERS_KEY]    = new Dictionary<Cars.CarBrand, List<Dealer>>();

            // Init our dicts with empty lists
            foreach(Cars.CarClass carClass in Enum.GetValues(typeof(Cars.CarClass))){
                dealerDict[dealerType][CLASS_TO_DEALERS_KEY][carClass] = new List<Dealer>();
            }
            foreach(Cars.CarBrand carBrand in Enum.GetValues(typeof(Cars.CarBrand))){
                dealerDict[dealerType][BRAND_TO_DEALERS_KEY][carBrand] = new List<Dealer>();
            }

            // Init, add ALL of the game's dealers here
            // Dealers may sell brands of cars, classes of cars, or specific classes of brands of cars
            foreach(object[] parameters in paramList){
                AddNewDealer((Dealer)Activator.CreateInstance(dealerType, parameters));
            }
        }
    }

    // Adds a new dealer to the 'database'
    public static void AddNewDealer(Dealer dealerToAdd){
        List<Cars.CarClass> dealerClasses                       = dealerToAdd.carClasses;
        List<Cars.CarBrand> dealerBrands                        = dealerToAdd.carBrands;
        string              dealerName                          = dealerToAdd.name;

        Type dealerType                                         = dealerToAdd.GetType();

        List<Dealer>                            dealers         = dealerDict[dealerType][ALL_DEALERS_KEY];
        Dictionary<string, Dealer>              nameToDealer    = dealerDict[dealerType][NAME_TO_DEALER_KEY];
        Dictionary<Cars.CarClass, List<Dealer>> classToDealers  = dealerDict[dealerType][CLASS_TO_DEALERS_KEY];
        Dictionary<Cars.CarBrand, List<Dealer>> brandToDealers  = dealerDict[dealerType][BRAND_TO_DEALERS_KEY];

        dealers.Add(dealerToAdd);
        nameToDealer[dealerName]                                = dealerToAdd;

        // For each car class
        foreach(Cars.CarClass carClass in dealerClasses){
            // Add the dealer to the list
            classToDealers[carClass].Add(dealerToAdd);
        }

        // For each car brand
        foreach(Cars.CarBrand carBrand in dealerBrands){
            // Add the dealer to the list
            brandToDealers[carBrand].Add(dealerToAdd);
        }
    }

    public static Dealer GetDealer(string name, Type dealerType){
        return dealerDict[dealerType][NAME_TO_DEALER_KEY][name];
    }

    public static List<Dealer> GetDealersForClass(Cars.CarClass dealerClass, Type dealerType){
        return dealerDict[dealerType][CLASS_TO_DEALERS_KEY][dealerClass];
    }
}