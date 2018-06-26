import React from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import Buttons from './Buttons/Buttons.js'
import Form from './Form/Form.js'
import ImportForm from './ImportForm/ImportForm.js'
import FileComp from './FileComp/FileComp.js'
import Uploader from './Uploader/Uploader.js'


const root = document.getElementById('root');
const param = JSON.parse(root.getAttribute('data-param'));
const savedData = JSON.parse(root.getAttribute('data-saved'));
let savedData1 = true;
let listSaved = [{ content: {}, color: {}, chars: {} }];

//2.do we need to apply turkish to english to all inputs in the page?? 
//3.dont forget to make ids or names for all forms inputs for the post request in new

let clicked = 0;

class App extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            forms: [],


        };

        this.AddNewHandle = this.AddNewHandle.bind(this);
        this.deleteHandle = this.deleteHandle.bind(this);
        this.ImportHandle = this.ImportHandle.bind(this);
        this.changeSenderHandler = this.changeSenderHandler.bind(this);
        this.changeCodeHandler = this.changeCodeHandler.bind(this);
        this.changeHandlerEng = this.changeHandlerEng.bind(this);
        this.changeHandlerTr = this.changeHandlerTr.bind(this);



    }

    //componentDidMount() {
    //    this.scrollToBottom();
    //}





    componentDidUpdate() {
        if (clicked === 1) {
            this.scrollToBottom();
            clicked = 0;
        }

    }

    scrollToBottom() {
        this.el.scrollIntoView({ behavior: 'smooth' });
    }


    changeHandlerTr(event, index) {

        console.log('event.target.value' + event.target.value);

        let flar = [...this.state.forms];
        if (event.target.value.length === 254)
            flar[index].color.tr = '#ff0000';
        else
            flar[index].color.tr = '#6c757d';


        flar[index].content.contenttr = event.target.value;
        flar[index].chars.tr = event.target.value.length;
        flar[index].content.contenttr = flar[index].content.contenttr
            .replace(/ş/g, 's')
            .replace(/Ş/g, 'S')
            .replace(/ç/g, 'c')
            .replace(/Ç/g, 'C')
            .replace(/ğ/g, 'g')
            .replace(/Ğ/g, 'G')
            .replace(/ı/g, 'i')
            .replace(/İ/g, 'I')
            .replace(/ö/g, 'o')
            .replace(/Ö/g, 'O')
            .replace(/ü/g, 'u')
            .replace(/Ü/g, 'U');


        this.setState({ forms: flar });

    }


    changeHandlerEng(event, index) {
        //console.log('index of current is' + event.target.index)
        console.log('event.target.value.lenght of current is' + event.target.value.length)

        let flar = [...this.state.forms];
        if (event.target.value.length === 254)
            flar[index].color.eng = '#ff0000';
        else
            flar[index].color.eng = '#6c757d';


        flar[index].content.contenteng = event.target.value;
        flar[index].chars.eng = event.target.value.length;
        this.setState({ forms: flar });

    }


    changeCodeHandler(event, index) {
        console.log("changed to " + event.target.value);
        console.log("this index is" + index);

        let selectedValue = event.target.value;
        let list = [...this.state.forms];



        //check if value is default
        if (selectedValue === 'default') {
            console.log("nothing should change ");
            list[index].content.contenteng = '';
            list[index].content.contenttr = '';
            list[index].valueCode = 'Default';
            list[index].chars = { eng: 0, tr: 0 };
            list[index].color.tr = '#6c757d';
            list[index].color.eng = '#6c757d';



            this.setState({ forms: list });
            return;
        }
        //send ajax request to server
        console.log('sending ajax request for id:' + event.target.value);
        axios.get('../../../../Api/Content?sender=' + this.state.forms[index].valueSender + '&code=' + event.target.value)
            .then(response => {
                console.log('data received is ' + JSON.stringify(response.data.English))

                list[index].content.contenteng = response.data.English;
                list[index].content.contenttr = response.data.Turkish;
                list[index].valueCode = selectedValue;
                list[index].chars = { eng: response.data.English.length, tr: response.data.Turkish.length };

                if (list[index].chars.eng === 254)
                    list[index].color.eng = '#ff0000';
                else
                    list[index].color.eng = '#6c757d';

                if (list[index].chars.tr === 254)
                    list[index].color.tr = '#ff0000';
                else
                    list[index].color.tr = '#6c757d';




                this.setState({ forms: list });
                console.log('response is' + JSON.stringify(response));
                console.log('after changing:' + JSON.stringify(this.state.forms));
            });

    }





    changeSenderHandler(event, index) {
        console.log("changed to " + event.target.value);
        console.log("this index is" + index);

        var value = event.target.value;
        let list = [...this.state.forms];

        //check if value is default
        if (value === 'default') {
            console.log("we are in bois ");
            list[index].content.contenteng = '';
            list[index].content.contenttr = '';
            list[index].valueSender = 'default';
            list[index].sender = [];
            list[index].chars = { eng: 0, tr: 0 };
            list[index].color.tr = '#6c757d';
            list[index].color.eng = '#6c757d';

            //extra line?
            this.setState({ forms: list });



            return;
        }
        //send ajax request to server
        console.log('sending ajax request for id:' + value);
        axios.get('../../../../Api/Codes?sender=' + value)
            .then(response => {

                list[index].sender = response.data;
                list[index].valueSender = value;
                list[index].valueCode = 'default';

                list[index].content.contenteng = '';
                list[index].content.contenttr = '';
                list[index].chars = { eng: 0, tr: 0 };

                list[index].color.tr = '#6c757d';
                list[index].color.eng = '#6c757d';



                this.setState({ forms: list });





                //var val = "Default";
                //var sel = document.getElementById('selected123');

                //var opts = sel.options;
                //for (var opt, j = 0; opt = opts[j]; j++) {
                //    if (opt.value == val) {
                //        sel.selectedIndex = j;
                //        break;
                //    }
                //}
                console.log('response is' + JSON.stringify(response));
                console.log('after changing:' + JSON.stringify(this.state.forms));
            });

    }


    deleteHandle(index) {
        console.log("deleting" + index);


        const list = [...this.state.forms];

        list.splice(index, 1);
        this.setState({ forms: list });


    }

    AddNewHandle() {
        console.log('add new handle');
        let persons = [...this.state.forms];
        console.log('current forms' + persons)




        if (this.state.forms.length < 1) {
            console.log('first condition');
            persons.push({ id: 1, empty: true, content: { contenteng: '', contenttr: '' }, chars: { eng: 0, tr: 0 }, color: { eng: '#6c757d', tr: '#6c757d' }, effect: 'unfold' })
            this.setState({ forms: persons })
        }
        else {
            if (this.state.forms.length === 1) {
                persons[0].effect = '';
                this.setState({ forms: persons })

            }
            console.log('second condition');
            console.log('add new handle with id ' + (this.state.forms[this.state.forms.length - 1].id + 1));

            persons.push({ id: this.state.forms[this.state.forms.length - 1].id + 1, empty: true, content: { contenteng: '', contenttr: '' }, chars: { eng: 0, tr: 0 }, color: { eng: '#6c757d', tr: '#6c757d' }, effect: 'new' })
            console.log('pushed object addnewhandle' + JSON.stringify(persons));

            if (this.state.forms.length >= 1) {
                clicked = 1;

            }

            this.setState({ forms: persons })



        }

        console.log('exiting add new handle');

    }

    ImportHandle() {
        let persons = [...this.state.forms];

        if (this.state.forms.length < 1) {
            //MAYBE I SHOULD RETRIEVE AND PLACE THE SENDERS HERE!
            persons.push({ id: 1, empty: false, sender: [], content: { contenteng: '', contenttr: '' }, chars: { eng: 0, tr: 0 }, color: { eng: '#6c757d', tr: '#6c757d' }, effect: 'unfold' })
            this.setState({ forms: persons, value: 'jjjj' })
            console.log('first condition');
        }
        else {
            if (this.state.forms.length === 1) {
                persons[0].effect = '';
                this.setState({ forms: persons })

            }
            console.log('second condition');

            persons.push({ id: this.state.forms[this.state.forms.length - 1].id + 1, empty: false, sender: [], content: { contenteng: '', contenttr: '' }, chars: { eng: 0, tr: 0 }, color: { eng: '#6c757d', tr: '#6c757d' }, effect: 'new' })
            console.log('pushed object importhandle' + JSON.stringify(persons));
            //is value necessary? or should be removed?

            if (this.state.forms.length >= 1) {

                clicked = 1;
            }

            this.setState({ forms: persons, value: 'jjjj' })

        }


    }



    render() {
        let forms = [];



        console.log('before null');

        if (savedData !== null && savedData1 === true) {

            console.log('inside null');

            //list[index].content.contenteng = '';
            //list[index].content.contenttr = '';
            //list[index].valueSender = 'default';
            //list[index].sender = [];
            //list[index].chars = { eng: 0, tr: 0 };
            //list[index].color.tr = '#6c757d';
            //list[index].color.eng = '#6c757d';



            //color: { eng: '#6c757d', tr: '#6c757d' }

            //let listSaved = [{ content: {}, color: {}, chars: {} }];

            savedData.map((form, index) => {
                if (index === 0) {
                    listSaved[index].id = 1;
                } else {
                    listSaved.push({ content: {}, color: {}, chars: {}});
                    listSaved[index].id = 1+listSaved[index-1].id;
                }
                listSaved[index].sender = form.Sender;
                listSaved[index].code = form.Code;
                listSaved[index].content.contenteng = form.EnContent;
                listSaved[index].content.contenttr = form.TrContent;
                listSaved[index].empty = true;
                listSaved[index].chars.eng = form.EnContent.length;
                listSaved[index].chars.tr = form.TrContent.length

                listSaved[index].effect = '';
                listSaved[index].color.eng = '#6c7675d';
                listSaved[index].color.tr = '#6c7675d';
            });
            console.log('before set state, listsaved is ' + listSaved);
            savedData1 = false;
            this.setState({ forms: listSaved });
            console.log('after set state, state is ' + this.state.forms);
            //stateFiles.push({ name: selectedFiles[i].name, size: selectedFiles[i].size, id: this.state.files.length === 0 ? 0 : this.state.files[this.state.files.length - 1].id + 1, fileContent: selectedFiles[i], default: 1 })
        }


        forms = this.state.forms.map((form, index) => {
            console.log(this.state.forms[index].content);
            console.log(form.id + 'key');
            //need to change empty to type
            if (form.empty === true)
                return (


                    <div ref={el => { this.el = el; }} >
                        <Form animation={this.state.forms[index].effect} changedTr={(event) => this.changeHandlerTr(event, index)} changedEng={(event) => this.changeHandlerEng(event, index)} click={() => this.deleteHandle(index)} key={form.id} newvalues={this.state.forms[index]} myIndex={index} />
                    </div>
                );
            else
                return (
                    <div ref={el => { this.el = el; }} >
                        <ImportForm animation={this.state.forms[index].effect} changedTr={(event) => this.changeHandlerTr(event, index)} changedEng={(event) => this.changeHandlerEng(event, index)} click={() => this.deleteHandle(index)} changeSender={(event) => this.changeSenderHandler(event, index)} changeCode={(event) => this.changeCodeHandler(event, index)} key={form.id} myParam={param} newvalues={this.state.forms[index]} myIndex={index} />
                    </div>

                );
        });




        return (

            <div>
                <Buttons clickAdd={this.AddNewHandle} clickImport={this.ImportHandle} />
                {forms}
            </div>

        );
    }
}


App.displayName = 'App';


















class Test1 extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            files: []

        };





        this.fileHandler = this.fileHandler.bind(this);
        this.removeHandle = this.removeHandle.bind(this);
        this.uploadHandle = this.uploadHandle.bind(this);



    }


    uploadHandle(event, index) {

        let stateFiles = [...this.state.files];
        stateFiles[index].default = 0;
        console.log('successfully passed stateFiles[index].default')
        this.setState({ files: stateFiles });
        console.log('successfully passed setstate')



        console.log('file sending is:' + this.state.files[index].name);

        var formData = new FormData();

        formData.append('hello', this.state.files[index]);
        $.ajax({
            url: '../../../../File/Upload',
            type: 'POST',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log('file has been sent and returning string from api is:' + response);
            },
            error: function (xhr, status, errorThrown) {
                console.log('it didnt work');
            }
        });


        // formData.append('hello', this.state.files[index]);
        //axios.post('../../../../Api/File', formData, {
        //    headers: {

        //        'Process-Data': 'false',
        //        'Content-Type': 'false'
        //    }
        //}).then(response => {


        //    console.log('file has been sent and returning string from api is:' + response);
        //});



        //axios.get('../../../../Api/Content?Id=' + event.target.value)



    }


    removeHandle(index) {
        //if file is already in the database
        if (this.state.files[index].default === 0) {
            //ajax request is sent to api in order to delete file from database
        }

        const list = [...this.state.files];

        list.splice(index, 1);
        this.setState({ files: list });

    }


    fileHandler(selectedFiles) {
        console.log('number of files is:' + selectedFiles.length);
        console.log('name of selected file is:' + selectedFiles[0].name);
        console.log(selectedFiles[0]);
        if (selectedFiles.length === 0) {
            return null;
        }
        else {

            let stateFiles = [...this.state.files];
            //console.log('current forms' + persons)


            for (var i = 0; i < selectedFiles.length; i++) {
                stateFiles.push({ name: selectedFiles[i].name, size: selectedFiles[i].size, id: this.state.files.length === 0 ? 0 : this.state.files[this.state.files.length - 1].id + 1, fileContent: selectedFiles[i], default: 1 })
            }
            this.setState({ files: stateFiles })






        }


    }


    render() {
        let files = [];

        files = this.state.files.map((file, index) => {
            //console.log(this.state.forms[index].content);
            //console.log(form.id + 'key');
            //need to change empty to type

            return (
                <div>
                    <FileComp clickUpload={(event) => this.uploadHandle(event, index)} clickRemove={() => this.removeHandle(index)} key={file.id} newFile={this.state.files[index]} default={this.state.files[index].default} />
                </div>
            );

        });
        //whats this for?
        //newvalues = { this.state.forms[index] }
        return (
            <div>
                <Uploader fileSelected={(e) => this.fileHandler(e.target.files)} />
                <div className="col-md-12">
                    <section id="todolist-cont" className="_todo-list">
                        <ul className="list-group _to-do-list-group sortable" id="task-list">
                            {files}
                        </ul>
                    </section>
                </div>
            </div>

        );
    }
}


Test1.displayName = 'Test1';


ReactDOM.render(<App />, document.getElementById('root'));
//ReactDOM.render(<Test1 />, document.getElementById('root2'));
