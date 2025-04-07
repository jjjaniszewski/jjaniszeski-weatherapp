import React, { useMemo, useState, useEffect, useCallback } from 'react';
import { useWeather } from './WeatherService';
import TemperatureChart from '../../charts/TemperatureChart';
import { formatTemperature, celsiusToKelvin } from '../../utils/temperature';
import { formatDateTime } from '../../utils/dateTime';
import { processWeatherData } from '../../utils/weatherDataProcessor';
import styles from './WeatherPage.module.scss';

const REFRESH_INTERVAL = 60; // seconds

const WeatherPage: React.FC = () => {
  const [secondsUntilRefresh, setSecondsUntilRefresh] = useState(REFRESH_INTERVAL);
  const [isInitialLoad, setIsInitialLoad] = useState(true);
  const { data: weatherRecords, loading, refetch } = useWeather();

  const handleRefresh = useCallback(() => {
    refetch();
    setSecondsUntilRefresh(REFRESH_INTERVAL);
  }, [refetch]);

  useEffect(() => {
    if (weatherRecords && isInitialLoad) {
      setIsInitialLoad(false);
    }
  }, [weatherRecords]);

  useEffect(() => {
    const timer = setInterval(() => {
      setSecondsUntilRefresh(prev => {
        if (prev <= 1) {
          handleRefresh();
          return REFRESH_INTERVAL;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [handleRefresh]);

  const { groupedData, globalMinTemp, globalMaxTemp } = useMemo(() => {
    return processWeatherData(weatherRecords);
  }, [weatherRecords]);

  if (isInitialLoad && loading) return <div className={styles.loading}>Loading...</div>;
  if (!weatherRecords) return <div className={styles.error}>Error loading weather data</div>;

  return (
    <div className={styles.weatherPage}>
      <div className={styles.headerContainer}>
        <h1>Weather Records by Location</h1>
        <div className={styles.refreshContainer}>
          <button onClick={handleRefresh} className={styles.refreshButton} disabled={loading}>
            {loading ? 'Refreshing...' : 'Refresh Now'}
          </button>
          <span className={styles.refreshTimer}>
            {loading && !isInitialLoad ? 'Refreshing...' : `Next refresh in: ${secondsUntilRefresh}s`}
          </span>
        </div>
      </div>
      <div className={styles.weatherGrid}>
        {Object.entries(groupedData).map(([key, data]) => {
          const latestRecord = data.records[data.records.length - 1];
          return (
            <div key={key} className={styles.weatherCard}>
              <h2>{data.city}, {data.country.toUpperCase()}</h2>
              <div className={styles.currentTemp}>
                Latest: {formatTemperature(celsiusToKelvin(latestRecord?.temperature ?? 0))}
                <span className={styles.updateTime}>
                  (Updated: {latestRecord ? formatDateTime(latestRecord.time.toISOString()) : 'N/A'})
                </span>
              </div>
              <div className={styles.chartContainer}>
                <TemperatureChart 
                  data={data.records}
                  minY={globalMinTemp}
                  maxY={globalMaxTemp}
                />
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default WeatherPage;