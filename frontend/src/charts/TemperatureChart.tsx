import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
  Title,
  Tooltip,
  Legend,
  Scale
} from 'chart.js';
import { formatChartTime } from '../utils/dateTime';

ChartJS.register(LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend);

interface TemperatureData {
    time: Date;
    min: number;
    max: number;
}

interface TemperatureChartProps {
    data: TemperatureData[];
    minY: number;
    maxY: number;
}

const TemperatureChart: React.FC<TemperatureChartProps> = ({ data, minY, maxY }) => {
  const chartData = {
    labels: data.map(d => formatChartTime(d.time)),
    datasets: [
      {
        label: 'Min Temperature',
        data: data.map(d => d.min),
        borderColor: 'rgb(53, 162, 235)',
        backgroundColor: 'rgba(53, 162, 235, 0.5)',
        tension: 0.6,
        cubicInterpolationMode: 'monotone' as const,
        borderWidth: 2.5,
        pointRadius: 0,
        pointHoverRadius: 0,
        pointStyle: false as const,
      },
      {
        label: 'Max Temperature',
        data: data.map(d => d.max),
        borderColor: 'rgb(255, 99, 132)',
        backgroundColor: 'rgba(255, 99, 132, 0.5)',
        tension: 0.6,
        cubicInterpolationMode: 'monotone' as const,
        borderWidth: 2.5,
        pointRadius: 0,
        pointHoverRadius: 0,
        pointStyle: false as const,
      }
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top' as const,
        align: 'end' as const,
        labels: {
          boxWidth: 15,
          padding: 15,
          font: {
            size: 11
          }
        }
      },
      tooltip: {
        mode: 'index' as const,
        intersect: false,
      }
    },
    scales: {
      x: {
        grid: {
          display: false
        },
        ticks: {
          font: {
            size: 10
          }
        }
      },
      y: {
        min: minY,
        max: maxY,
        grid: {
          color: 'rgba(0, 0, 0, 0.1)'
        },
        ticks: {
          font: {
            size: 10
          },
          callback: function(this: Scale<any>, value: number | string) {
            return `${Number(value).toFixed(1)}°C`;
          }
        }
      }
    },
    interaction: {
      mode: 'nearest' as const,
      axis: 'x' as const,
      intersect: false
    }
  };

  return <Line data={chartData} options={options} />;
};

export default TemperatureChart;