export interface CarData {
    id: number;
    brand: string;
    model: string;
    bodyTypeId: number;
    yearOfProduction: number;
    fuelId: number;
    vin: string;
    colorId: number;
    mileage: number;
    gearboxId: number;
    propulsionId: number;
    capacity: number;
    power: number;
    numberOfDoors: number;
    numberOfSeats: number;
    rightSideSteeringWheel: boolean;
    registration?: string;
    price: number;
    pricePerHour: number;
}
