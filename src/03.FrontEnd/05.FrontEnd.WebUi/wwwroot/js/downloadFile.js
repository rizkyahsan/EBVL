function downloadFile(filename, contentType, base64Data) {
    const binaryString = atob(base64Data);
    const bytes = new Uint8Array(binaryString.length);

    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    const blob = new Blob(
        [bytes],
        { type: contentType });

    const exportUrl = URL.createObjectURL(blob);
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.click();

    document.body.removeChild(a);

    URL.revokeObjectURL(exportUrl);
}
