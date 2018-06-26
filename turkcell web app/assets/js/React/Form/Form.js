import React from 'react';
import ReactDOM from 'react-dom';
import './Form.css';


var divStyle = {

    backgroundColor: '#f1f7fc', width: '80%', marginTop: '15px', marginBottom: '15px'
};

const Form = (props) => {

    console.log('PROPS.ANIMATION'+JSON.stringify(props.animation));
    var classs = 'container ' + props.animation;
    return (
        
           
            <div className={classs} style={divStyle}>
            <div className="row align-items-center">
                <div className="col-5 col-sm-5 col-md-3 col-lg-4 col-xl-5 offset-xl-1"><p className="form-control-plaintext">Sender:</p> </div>
                <div className="col-4 col-sm-3 col-md-2 col-lg-2 col-xl-2 offset-3 offset-sm-4 offset-md-7 offset-lg-6 offset-xl-4"><button onClick={props.click} className="btn btn-danger btn-block" type="button"><i className="fa fa-trash" style={{ fontSize: '25px' }}></i></button></div>
            </div>
            <div className="row">
                <div className="col-xl-7 offset-xl-1"><input value={props.newvalues.sender} name={"Panel4.SMS["+props.myIndex+"].Sender"} type="text" onChange={props.change} style={{ width: '80%' }} /></div>
            </div>
            <div className="row align-items-center">
                <div className="col-xl-5 offset-xl-1"><p className="form-control-plaintext">Code:</p></div>
            </div>
            <div className="row align-items-center">
                <div className="col-xl-7 offset-xl-1"><input value={props.newvalues.code} name={"Panel4.SMS[" + props.myIndex + "].Code"} type="text" style={{ width: '80%' }} /></div>
            </div>
            <div className="row">
                <div className="col-xl-5 offset-xl-1"><p className="form-control-plaintext">Content (EN):</p></div>
            </div>
            <div className="row">
                <div className="col-xl-11 offset-xl-1"><textarea value={props.newvalues.content.contenteng} name={"Panel4.SMS[" + props.myIndex + "].EnContent"} onChange={props.changedEng} wrap="hard" maxlength="254" style={{ width: '80%', minHeight: '105px' }}></textarea></div>
                <div className="col-3 col-sm-2 col-md-1 col-xl-1 offset-7 offset-sm-8 offset-md-8 offset-lg-8 offset-xl-9"><small style={{ color: props.newvalues.color.eng }} className="form-text">{props.newvalues.chars.eng}/254</small></div>
            </div>
            <div className="row">
                <div className="col-xl-5 offset-xl-1"><p className="form-control-plaintext">Content (TR):</p></div>
            </div>
            <div className="row">
                <div className="col-xl-11 offset-xl-1"><textarea value={props.newvalues.content.contenttr} name={"Panel4.SMS[" + props.myIndex + "].TrContent"} onChange={props.changedTr} value={props.newvalues.content.contenttr} wrap="hard" maxlength="254" style={{ width: '80%', minHeight: '105px' }}></textarea></div>
                <div className="col-3 col-sm-2 col-md-1 col-xl-1 offset-7 offset-sm-8 offset-md-8 offset-lg-8 offset-xl-9"><small style={{ color: props.newvalues.color.tr }} className="form-text">{props.newvalues.chars.tr}/254</small></div>
            </div>
           
        </div>
    
    );
};
Form.displayName = 'Form'

//export { Form } from './Form.js'
export default Form;