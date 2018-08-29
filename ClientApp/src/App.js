import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { Get } from './utils/HttpRequest'
import { Portal } from './components/Account/Portal';

export default class App extends Component {
    displayName = App.name

    constructor(props) {
        super(props);

        this.state = {
            user: null
        };

        Get('/Account/GetUserInfoAsync')
            .then(data => data.json())
            .then(data => {
                this.setState({ user: data, loading: false });
                console.log(data);
            });

        // fetch('/Account/GetUserInfoAsync', { credentials: "same-origin" })
        //     .then(data => data.json())
        //     .then(data => {
        //         this.setState({ user: data, loading: false });
        //         console.log(data);
        //     });
    }

    setTitle(title) {
        document.title = title + " - SYSU MSC";
    }

    render() {
        return (
            <div>
                <Layout user={this.state.user}>
                    <Route exact path='/' render={() => { this.setTitle('主页'); return <Home user={this.state.user} />; }} />
                    <Route path='/counter' render={() => { this.setTitle('Counter'); return <Counter user={this.state.user} />; }} />
                    <Route path='/fetchdata' render={() => { this.setTitle('FetchData'); return <FetchData user={this.state.user} />; }} />
                    <Route path='/portal' render={() => { this.setTitle('我的账户'); return <Portal user={this.state.user} />; }}></Route>
                </Layout>
            </div>
        )
    }
}
