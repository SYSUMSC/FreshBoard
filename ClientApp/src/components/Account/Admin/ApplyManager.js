import React, { Component } from "react";

export class ApplyManager extends Component {
    displayName = ApplyManager.name

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