import React, { Component } from 'react';
import * as signalR from '@aspnet/signalr';

export class Crack extends Component {
    displayName = Crack.name
    
    constructor(props) {
        super(props);
        let connection = new signalR.HubConnectionBuilder().withUrl("/hub").build();

        connection.start();
    }

    render() {
        return null;
    }
}