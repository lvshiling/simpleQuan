import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { StockList } from "./components/StockList";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/stocks',
    element: <StockList />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  }
];

export default AppRoutes;
