import React from 'react';
import ReactDOM from 'react-dom';





const FileComp = (props) => {


    console.log('props.default is: ' + props.default)
    let deleteButton = null;

    if (props.default === 1) {
        deleteButton = (<button onClick={props.clickUpload} className="btn btn-light _todo-upload" type="button" data-task-control="upload" > <i className="fa fa-cloud-upload"></i> </button >)
    }
    else {
        deleteButton = (<button onClick={props.clickUpload} className="btn btn-light _todo-upload" type="button" data-task-control="upload" > <i class="fa fa-check" aria-hidden="true"></i>
        </button >)

    }






    return (
        <div>
            <li className="list-group-item _task">
                <span className="_todo-text" style={{ width: '33.3%', float: 'left' }}>Name: {props.newFile.name}</span>
                <span className="_todo-text">Size:  {parseFloat(props.newFile.size / Math.pow(1024, 2)).toFixed(2)}MB</span>
                <div className="btn-group pull-right _task-controls" role="group">
                    <button onClick={props.clickRemove} style={{ width: '24px !important', height: '24px !important' }} className="btn btn-light _todo-remove" type="button" data-task-control="remove"> <i className="fa fa-trash"></i></button>
                    {deleteButton}
                </div>
                <div className="clearfix"></div>
            </li>
        </div>



    );
};






FileComp.displayName = 'FileComp'


export default FileComp;