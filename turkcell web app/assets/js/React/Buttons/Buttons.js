import React from 'react';
import ReactDOM from 'react-dom';
import '../Form/Form.css';

const Buttons = (props) => {

    return (
        <div className="row no-gutters justify-content-center align-items-center">
            <div className="col-5 col-sm-5 col-md-3 col-lg-3 col-xl-2 offset-lg-0"><button onClick={props.clickAdd}  className="btn btn-primary btn-block" type="button">Add New</button></div>
            <div className="col-5 col-sm-5 col-md-3 col-lg-3 col-xl-2 offset-lg-1"><button onClick={props.clickImport} className="btn btn-primary btn-block" type="button">Import</button></div>
        </div>

    );
};
Buttons.displayName = 'zzz'

export { Buttons } from './Buttons.js'
export default Buttons;