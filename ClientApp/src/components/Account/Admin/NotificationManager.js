import React, { Component } from "react";

export class NotificationManager extends Component {
    displayName = NotificationManager.name

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                {this.displayName}
            </div>
        );
    }
}