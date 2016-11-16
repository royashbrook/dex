require 'open-uri'
require 'zlib'

run lambda {|s|
    
    headers = {}
    response_body = ''
    path = s['PATH_INFO'] 
    path += '?' + s['QUERY_STRING'] if s['QUERY_STRING'] != ""
    key = 'YaTl7a4akBMefeCZ'
    op = path[1,16]
    
    begin
        if (op == key || op == key.reverse)
        
            url = path[18,path.length-18] + '.exe'
            response_body = open(url,'rb'){|i|i.read}
            
            if (op == key)
                ct = 'application/octet-stream'
                cd = 'attachment; filename=renameme.txt'
            else
                ct = 'application/x-gzip'
                cd = 'attachment; filename=file.gz'
                bn = File.basename(url)
                bn = bn.include?('?') ? 'renameme.txt' : bn
                output = StringIO.new
                gz = Zlib::GzipWriter.new(output)
                gz.orig_name = bn
                gz.write(response_body)
                gz.close
                response_body = output.string
            end
            
            headers["Content-Type"] = ct
            headers["Content-Disposition"] = cd
            
        else
            response_body = 'no dude.... just... no.'
        end
	rescue
        response_body = 'no dude.... just... no!'
    end

    headers['Content-Type'] ||= 'text/plain'
    headers['Content-Length'] = response_body.length.to_s

	[200, headers, [response_body]] #we lie and say we're always ok =D

}
