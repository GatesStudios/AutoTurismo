using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public abstract class Dealer : UnityEngine.Object
{
    // Name is unique
    public string                               dealerName;
    public SerializableList<Cars.CarClass>      carClasses;
    public SerializableList<Cars.CarBrand>      carBrands;
    public SerializableList<Purchasable>        products;

    public Dealer(string nameParam, SerializableList<Cars.CarClass> carClassesParam, SerializableList<Cars.CarBrand> carBrandsParam){
        dealerName      = nameParam;
        carClasses      = carClassesParam;
        carBrands       = carBrandsParam;
        products        = new SerializableList<Purchasable>();
    }

    public Sprite GetSprite(){
        string imageName = dealerName.Replace(" ", "") + "_Dealer";
        return Resources.Load<Sprite>("Images/Dealers/" + imageName);
    }

    public void AddProduct(Purchasable product){
        if(!products.Contains(product)){
            products.Add(product);
        }
    }
}

public class CarDealer : Dealer
{
    public CarDealer(string nameParam, SerializableList<Cars.CarClass> carClassesParam, SerializableList<Cars.CarBrand> carBrandsParam) : base(nameParam, carClassesParam, carBrandsParam) {

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
    public EntryPassDealer(string nameParam,  SerializableList<Cars.CarClass> carClassesParam, SerializableList<Cars.CarBrand> carBrandsParam) : base(nameParam, carClassesParam, carBrandsParam) {
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
    // DEVS: change the contents of these 3 collections to add new types of dealers (whos data is generated by the below array)
    public static   List<SerializableType>                              dealerTypes             = new List<SerializableType>() { new SerializableType(typeof(CarDealer)), new SerializableType(typeof(EntryPassDealer)) } ;
    public static   Dictionary<Type, SerializableType>                  dealerTypeToSerialized  = new Dictionary<Type, SerializableType>() { {typeof(CarDealer), dealerTypes[0]}, {typeof(EntryPassDealer), dealerTypes[1]} } ;

    private static  List<Type>                                          dealerRealTypes         = new List<Type>() { typeof(CarDealer), typeof(EntryPassDealer) } ;

    // Holds ALL dealers in the database
    private const   string                                              ALL_DEALERS_KEY         = "dealers";
    // Will match unique name to dealer
    private const   string                                              NAME_TO_DEALER_KEY      = "nameToDealer";
    // Will match class to list of dealers who sell that class
    private const   string                                              CLASS_TO_DEALERS_KEY    = "classToDealers";
    // Will match brand to list of dealers who sell that brand
    private const   string                                              BRAND_TO_DEALERS_KEY    = "brandToDealers";

    private static  Dictionary<Type, Dictionary<string, dynamic>>    dealerDict {get; set;}

    static Dealers(){
        dealerDict                                          = new Dictionary<Type, Dictionary<string, dynamic>>();

        foreach(Type dealerType in dealerRealTypes){
            dealerDict[dealerType]                          = new Dictionary<string, dynamic>();
            dealerDict[dealerType][ALL_DEALERS_KEY]         = new List<Dealer>();
            dealerDict[dealerType][NAME_TO_DEALER_KEY]      = new Dictionary<string, Dealer>();
            dealerDict[dealerType][CLASS_TO_DEALERS_KEY]    = new Dictionary<Cars.CarClass, List<Dealer>>();
            dealerDict[dealerType][BRAND_TO_DEALERS_KEY]    = new Dictionary<Cars.CarBrand, List<Dealer>>();

            // Init our dicts with empty lists


            // Needed else will error out on the first AddNewDealer
            dealerDict[dealerType][BRAND_TO_DEALERS_KEY][Cars.CarBrand.None] = new List<Dealer>();

            // Init, add ALL of the game's dealers here
            // Dealers may sell brands of cars, classes of cars

            foreach(Cars.CarClass carClass in Enum.GetValues(typeof(Cars.CarClass))){
                dealerDict[dealerType][CLASS_TO_DEALERS_KEY][carClass] = new List<Dealer>();
                AddNewDealer((Dealer)Activator.CreateInstance(dealerType, new object[]
                    { Cars.classToString[carClass],     new SerializableList<Cars.CarClass> {carClass},             new SerializableList<Cars.CarBrand> {Cars.CarBrand.None} } ));
            }
            foreach(Cars.CarBrand carBrand in Enum.GetValues(typeof(Cars.CarBrand))){
                dealerDict[dealerType][BRAND_TO_DEALERS_KEY][carBrand] = new List<Dealer>();
                AddNewDealer((Dealer)Activator.CreateInstance(dealerType, new object[]
                    { carBrand.ToString(),              new SerializableList<Cars.CarClass> {Cars.CarClass.None},   new SerializableList<Cars.CarBrand> {carBrand} } ));
            }
        }
    }

    // Adds a new dealer to the 'database'
    public static void AddNewDealer(Dealer dealerToAdd){
        SerializableList<Cars.CarClass> dealerClasses           = dealerToAdd.carClasses;
        SerializableList<Cars.CarBrand> dealerBrands            = dealerToAdd.carBrands;
        string              dealerName                          = dealerToAdd.dealerName;

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

    public static Dealer GetDealer(string dealerNameParam, Type dealerType){
        return dealerDict[dealerType][NAME_TO_DEALER_KEY][dealerNameParam];
    }

    public static List<Dealer> GetDealersForClass(Cars.CarClass dealerClass, Type dealerType){
        return dealerDict[dealerType][CLASS_TO_DEALERS_KEY][dealerClass];
    }
}