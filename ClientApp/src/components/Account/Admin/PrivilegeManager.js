import React, { Component } from "react";

export class PrivilegeManager extends Component {
    displayName = PrivilegeManager.name

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