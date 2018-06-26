import React from 'react';
import ReactDOM from 'react-dom';
import '../Form/Form.css';



var divStyle = {

    backgroundColor: '#f1f7fc', width: '80%', marginTop: '15px', marginBottom: '15px'
};



const ImportForm = (props) => {
    let codes = null;
    let content = null;
    console.log('props.newvalues.sender is: ' + JSON.stringify(props.newvalues.sender));
    console.log('\nprops.myparam is: ' + JSON.stringify(props.myParam));
    console.log('\n props.newvalues.content.contenteng: ' + JSON.stringify(props.newvalues.content.contenteng) +
        '\n props.newvalues.content.contenttr: ' + JSON.stringify(props.newvalues.content.contenttr));



    const senders = props.myParam.map((eachSender, index) => {
        return (<option value={eachSender.Title}>{eachSender.Title}</option>);
    });

    if (props.newvalues.sender.length > 0) {
        codes = props.newvalues.sender.map((eachSender, index) => {
            return (<option value={eachSender.Code}>{eachSender.Code}   -   {eachSender.Explaination}</option>);
        });
      
    }
    else {
        codes = null;
    }


    var classs = 'container ' + props.animation;

    return (



        <div className={classs} style={divStyle}>
            <div className="row align-items-center">
                <div className="col-5 col-sm-5 col-md-3 col-lg-4 col-xl-5 offset-xl-1"><p className="form-control-plaintext">Sender:</p></div>
                <div className="col-4 col-sm-3 col-md-2 col-lg-2 col-xl-2 offset-3 offset-sm-4 offset-md-7 offset-lg-6 offset-xl-4"><button onClick={props.click} className="btn btn-danger btn-block" type="button"><i className="fa fa-trash" style={{ fontSize: '25px' }}></i></button></div>
            </div>
            <div className="row align-items-center">
                <div className="col-xl-7 offset-xl-1 align-self-center">
                    <div className="form-group">
                        <select name={"Panel4.SMS[" + props.myIndex + "].Sender"} value={props.newvalues.valueSender}  onChange={props.changeSender} required="" style={{ width: '80%' }}>
                            <option value="default">Please select an option</option>
                            {senders}
                        </select>
                    </div>
                </div>
            </div>
            <div className="row align-items-center">
                <div className="col-xl-5 offset-xl-1"><p className="form-control-plaintext">Code:</p></div>
            </div>
            <div className="row align-items-center">
                <div className="col-xl-7 offset-xl-1">
                    <div className="form-group">
                        <select name={"Panel4.SMS[" + props.myIndex + "].Code"} value={props.newvalues.valueCode} id="selected123" onChange={props.changeCode} required="" style={{ width: '80%' }}>
                            <option value="default">Please select an option</option>
                            {codes}
                        </select>
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col-xl-5 offset-xl-1"><p className="form-control-plaintext">Content (EN):</p></div>
            </div>
            <div className="row">
                <div className="col-xl-11 offset-xl-1">
                    <textarea name={"Panel4.SMS[" + props.myIndex + "].EnContent"} onChange={props.changedEng} value={props.newvalues.content.contenteng} wrap="hard" maxlength="254" style={{ width: '80%', minHeight: '105px' }}></textarea>
                </div>
                <div className="col-3 col-sm-2 col-md-1 col-xl-1 offset-7 offset-sm-8 offset-md-8 offset-lg-8 offset-xl-9">
                    <small className="form-text" style={{ color: props.newvalues.color.eng }}>{props.newvalues.chars.eng}/254</small>
                </div>
            </div>
            <div className="row">
                <div className="col-xl-5 offset-xl-1"><p className="form-control-plaintext">Content (TR):</p></div>
            </div>
            <div className="row">
                <div className="col-xl-11 offset-xl-1">
                    <textarea name={"Panel4.SMS[" + props.myIndex + "].TrContent"} onChange={props.changedTr} value={props.newvalues.content.contenttr} wrap="hard" maxlength="254" style={{ width: '80%', minHeight: '105px' }}>

                    </textarea>
                </div>
                <div className="col-3 col-sm-2 col-md-1 col-xl-1 offset-7 offset-sm-8 offset-md-8 offset-lg-8 offset-xl-9">
                    <small className="form-text" style={{ color: props.newvalues.color.tr }}>{props.newvalues.chars.tr}/254</small>
                </div>
            </div>


        </div>


    );



}
ImportForm.displayName = 'importedForm'















export { ImportForm } from './ImportForm.js'
export default ImportForm;