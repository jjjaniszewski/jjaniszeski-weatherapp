import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import WeatherPage from '../features/weather/WeatherPage';

const AppRoutes = () => (
  <Router>
    <Routes>
      <Route path="/" element={<WeatherPage />} />
    </Routes>
  </Router>
);

export default AppRoutes;