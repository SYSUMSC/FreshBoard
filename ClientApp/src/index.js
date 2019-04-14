
import 'bootstrap/dist/css/bootstrap.min.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import registerServiceWorker from './registerServiceWorker';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

var num = 10;
var obj = document.getElementById('preloader');
var st = setInterval(function () {
    num--;
    obj.style.opacity = num / 10;
    if (num <= 0) {
        obj.remove();
        clearInterval(st);
    }
}, 30);

if (window.location.href.toLowerCase().indexOf('/hackathon') === -1) {
    ReactDOM.render(
        <BrowserRouter basename={baseUrl}>
            <App />
        </BrowserRouter>,
        rootElement);

    //registerServiceWorker();
}
