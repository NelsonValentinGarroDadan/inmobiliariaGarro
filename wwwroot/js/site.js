// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Formularios ABM
const formDelete = document.getElementById("delete");
const formCreate = document.getElementById("create");
const formUpdate = document.getElementById("update");


const enviarFormalario = async (e,str)=>{
    e.preventDefault();
    const clave = document.getElementById("clave");
    const claveC = document.getElementById("claveConfirm");
    if(clave && claveC){
        if(clave.value != claveC.value){
            Swal.fire({
                title:"Las claves no coinciden",
                icon: "warning",
                confirmButtonText: "Aceptar"
            });
            return;
        }
    }
    let result = await Swal.fire({
        title:`${str}`,
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Aceptar"
    });
    if(result.isConfirmed) e.target.submit();
   
}

if(formDelete) formDelete.addEventListener("submit", (e)=>{enviarFormalario(e,"¿Estas seguro que deseas eliminar esta entidad?")});
if(formCreate) formCreate.addEventListener("submit", (e)=>{enviarFormalario(e,"¿Estas seguro que deseas crear esta entidad?")});
if(formUpdate) formUpdate.addEventListener("submit", (e)=>{enviarFormalario(e,"¿Estas seguro que deseas actualizar esta entidad?")});

// Animacion de navbar

const links = document.getElementsByClassName("nav-link");
const title = document.getElementById("tituloPag");

let url = window.location.pathname;
for(i=0;i<links.length;i++){
    let str = links[i].innerHTML;
    for(j=0;j<str.length-4;j++){
        let suma = str.charAt(j)+str.charAt(j+1)+str.charAt(j+2)+str.charAt(j+3);
        if(suma == "</i>"){
            str = str.substring(j+5);
            break;
        }
    }
    
    if(url=="/"){
        let c = links.length == 2 ? 1 : 2;
        title.innerHTML = links[c].innerHTML;
        links[0].className += " active";
        break;
    }
    if(url.includes(str)){
        title.innerHTML = links[i].innerHTML;
        links[i].className += " active";
        break;
    }
}
//#0000002b;