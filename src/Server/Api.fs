module Api

open DataAccess
open FSharp.Data.UnitSystems.SI.UnitNames
open Shared

let london = {
    Latitude = 51.5074
    Longitude = 0.1278
}

let getDistanceFromLondon postcode = async {
    if not (Validation.isValidPostcode postcode) then
        failwith "Invalid postcode"

    let! location = getLocation postcode
    let distanceToLondon = getDistanceBetweenPositions location.LatLong london

    return {
        Postcode = postcode
        Location = location
        DistanceToLondon = (distanceToLondon / 1000.<meter>)
    }
}

let getCrimeReport postcode = async {
    if not (Validation.isValidPostcode postcode) then
        failwith "Invalid postcode"

    let! location = getLocation postcode
    let! reports = getCrimesNearPosition location.LatLong

    let crimes =
        reports
        |> Array.countBy (fun r -> r.Category)
        |> Array.sortByDescending snd
        |> Array.map (fun (k, c) -> { Crime = k; Incidents = c })

    return crimes
}

let private asWeatherResponse (weather: Weather.OpenMeteoCurrentWeather.CurrentWeather) = {
    WeatherType = weather.Weathercode |> WeatherType.FromCode
    Temperature = float weather.Temperature
}

let getWeather postcode = async {
    if not (Validation.isValidPostcode postcode) then
        failwith "Invalid postcode"

    let! location = getLocation postcode
    let! weather = getWeatherForPosition location.LatLong
    let weatherResponse = weather |> asWeatherResponse
    return weatherResponse }


let dojoApi = {
    GetDistance = getDistanceFromLondon

    (* Task 1.1 CRIME: Bind the getCrimeReport function to the GetCrimes method to
         return crime data. Use the above GetDistance field as an example. *)
    GetCrimes = getCrimeReport  //fun postcode -> async { return Array.empty }

(* Task 4.2 WEATHER: Hook up the weather endpoint to the getWeather function. *)
    GetWeather = getWeather
}