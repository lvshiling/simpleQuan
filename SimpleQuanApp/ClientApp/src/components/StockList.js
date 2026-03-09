import React, { Component } from 'react';

export class StockList extends Component {
  static displayName = StockList.name;

  constructor(props) {
    super(props);
    this.state = { stocks: [], loading: true };
  }

  componentDidMount() {
    this.populateStockData();
  }

  static renderStocksTable(stocks) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>代码</th>
            <th>名称</th>
            <th>最新价</th>
            <th>涨跌幅(%)</th>
            <th>成交额 (亿)</th>
          </tr>
        </thead>
        <tbody>
          {stocks.map(stock =>
            <tr key={stock.code}>
              <td>{stock.code}</td>
              <td>{stock.name}</td>
              <td>{stock.price.toFixed(2)}</td>
              <td style={{ color: stock.changePercent >= 0 ? 'red' : 'green' }}>
                {stock.changePercent.toFixed(2)}%
              </td>
              <td>{(stock.turnover / 100000000).toFixed(2)}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : StockList.renderStocksTable(this.state.stocks);

    return (
      <div>
        <h1 id="tabelLabel">A股成交额前100名</h1>
        <p>实时统计当前成交额前100的个股。</p>
        {contents}
      </div>
    );
  }

  async populateStockData() {
    try {
      const response = await fetch('api/stock/top100');
      const data = await response.json();
      this.setState({ stocks: data, loading: false });
    } catch (error) {
        console.error("Error fetching data:", error);
        this.setState({ loading: false });
    }
  }
}
