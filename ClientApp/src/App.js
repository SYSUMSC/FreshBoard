import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

export default class App extends Component {
    displayName = App.name

    setTitle(title) {
        document.title = title + " - SYSU MSC";
    }

    render() {
        return (
            <Layout>
                <Route exact path='/' render={() => { this.setTitle('主页'); return <Home />; }} />
                <Route path='/counter' render={() => { this.setTitle('Counter'); return <Counter />; }} />
                <Route path='/fetchdata' render={() => { this.setTitle('FetchData'); return <FetchData />; }} />
            </Layout>
        );
    }
}
