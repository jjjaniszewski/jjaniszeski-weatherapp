/**
 * Converts temperature from Kelvin to Celsius
 * @param kelvin Temperature in Kelvin
 * @returns Temperature in Celsius
 */
export const kelvinToCelsius = (kelvin: number): number => {
    return kelvin - 273.15;
};

/**
 * Converts temperature from Celsius to Kelvin
 * @param celsius Temperature in Celsius
 * @returns Temperature in Kelvin
 */
export const celsiusToKelvin = (celsius: number): number => {
    return celsius + 273.15;
};

/**
 * Formats temperature for display with the Celsius symbol
 * @param kelvin Temperature in Kelvin
 * @returns Formatted temperature string in Celsius with symbol
 */
export const formatTemperature = (kelvin: number): string => {
    return `${kelvinToCelsius(kelvin).toFixed(1)}°C`;
}; 