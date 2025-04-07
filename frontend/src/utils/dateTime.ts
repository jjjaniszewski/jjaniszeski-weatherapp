/**
 * Formats a date for chart display, showing only the hour
 * @param date The date to format
 * @returns Formatted time string (HH:mm)
 */
export const formatChartTime = (date: Date): string => {
    return date.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    });
};

/**
 * Formats a date for display with date and time
 * @param dateStr ISO date string
 * @returns Formatted date and time string
 */
export const formatDateTime = (dateStr: string): string => {
    const date = new Date(dateStr);
    return date.toLocaleString('en-US', {
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    });
};

/**
 * Checks if a date is within the last 24 hours
 * @param date The date to check
 * @returns boolean indicating if the date is within the last 24 hours
 */
export const isWithinLast24Hours = (date: Date): boolean => {
    const twentyFourHoursAgo = new Date();
    twentyFourHoursAgo.setHours(twentyFourHoursAgo.getHours() - 86);
    return date >= twentyFourHoursAgo;
}; 