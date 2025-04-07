import { useFetch } from '../../hooks/useFetch';

export interface WeatherRecordDto {
    id: string;
    city: string;
    country: string;
    dateUtc: string;
    requestDateUtc: string;
    temperature: number;
    minTemperature: number;
    maxTemperature: number;
}

export const useWeather = () => {
    const url = `http://localhost:5115/WeatherRecord`;
    const { data, loading, refetch } = useFetch<WeatherRecordDto[]>(url);
    return { data, loading, refetch };
};