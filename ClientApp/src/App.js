import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

export default class App extends Component {
  displayName = App.name

  render() {
    const setTitle = title => () => document.title = 'SYSU MSC - ' + title;
    return (
      <Layout>
        <Route exact path='/' component={Home} onEnter={setTitle('主页')} />
        <Route path='/counter' component={Counter} onEnter={setTitle('Counter')} />
        <Route path='/fetchdata' component={FetchData} onEnter={setTitle('Fetch Data')} />
      </Layout>
    );
  }
}
