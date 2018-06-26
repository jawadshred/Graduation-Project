import React from 'react';
import ReactDOM from 'react-dom';



const Uploader = (props) => {


    return (
        <div className="row no-gutters">
            <div className="col">
                <div className="dashed_upload" style={{ height: '250px' }}>
                    <div className="wrapper">
                        <div className="drop">
                            <div className="cont">
                                <i className="fa fa-cloud-upload"></i>
                                <div className="tit">
                                    Drag & Drop
                                            </div>
                                <div className="desc">
                                    or
                                            </div>
                                <div className="browse">
                                    click here to browse
                                            </div>
                            </div>
                            <output id="list"></output>
                            <input  id="files" multiple name="UploadFiles" type="file" onChange={props.fileSelected} />
                        </div>
                    </div>

                </div>
            </div>
        </div>




    );
};



Uploader.displayName = 'Uploader'


export default Uploader;