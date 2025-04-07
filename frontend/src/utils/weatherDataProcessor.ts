import { kelvinToCelsius } from './temperature';
import { isWithinLast24Hours } from './dateTime';

export interface WeatherRecord {
    time: Date;
    min: number;
    max: number;
    temperature: number;
}

export interface LocationWeatherData {
    city: string;
    country: string;
    records: WeatherRecord[];
}

export interface ProcessedWeatherData {
    groupedData: { [key: string]: LocationWeatherData };
    globalMinTemp: number;
    globalMaxTemp: number;
}

export const processWeatherData = (weatherRecords: any[] | null): ProcessedWeatherData => {
    if (!weatherRecords) return { groupedData: {}, globalMinTemp: 0, globalMaxTemp: 0 };

    let minTemp = Infinity;
    let maxTemp = -Infinity;

    const grouped = weatherRecords.reduce<{ [key: string]: LocationWeatherData }>((acc, record) => {
        const key = `${record.city}-${record.country}`;
        const recordDate = new Date(record.dateUtc);

        // Only include records from the last 24 hours
        if (!isWithinLast24Hours(recordDate)) {
            return acc;
        }

        const minTempCelsius = kelvinToCelsius(record.minTemperature);
        const maxTempCelsius = kelvinToCelsius(record.maxTemperature);

        // Update global min/max
        minTemp = Math.min(minTemp, minTempCelsius);
        maxTemp = Math.max(maxTemp, maxTempCelsius);

        if (!acc[key]) {
            acc[key] = {
                city: record.city,
                country: record.country,
                records: [],
            };
        }

        acc[key].records.push({
            time: recordDate,
            min: minTempCelsius,
            max: maxTempCelsius,
            temperature: kelvinToCelsius(record.temperature),
        });

        return acc;
    }, {});

    // Sort records for each location by time
    Object.values(grouped).forEach(locationData => {
        locationData.records.sort((a, b) => a.time.getTime() - b.time.getTime());
    });

    // Add 10% margin to min/max
    const range = maxTemp - minTemp;
    const margin = range * 0.1;
    const adjustedMin = Math.max(0, minTemp - margin); // Don't go below 0 if data is positive
    const adjustedMax = maxTemp + margin;

    return {
        groupedData: grouped,
        globalMinTemp: adjustedMin,
        globalMaxTemp: adjustedMax
    };
}; 