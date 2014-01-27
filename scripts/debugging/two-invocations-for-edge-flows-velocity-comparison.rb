
class EdgeData
	
	attr_accessor :flow, :velocity

end

def do_one_analysis
	output = `../../it.unifi.dsi.stlab.networkreasoner.console.emacs/bin/Debug/it.unifi.dsi.stlab.networkreasoner.console.emacs.exe < \
		../../it.unifi.dsi.stlab.networkreasoner.console.emacs/bin/Debug/emacs-buffers-examples/big-network-for-integration.org`

	splitted = output.split "\n"
	edge_results_start_index = splitted.find_index do |aString| 
			aString == "| EDGE ID | LINK | FLOW | VELOCITY |" 
		end
	edge_results_start_index += 2

	
	edge_results_end_index = splitted.find_index do |aString| 
			aString == "* Dot representations"
		end
	edge_results_end_index -= 4

#	output_string = "edge result table starts at #{edge_results_start_index}
#			and ends at #{edge_results_end_index}"
#	puts output_string 

	edge_results_table = splitted.slice edge_results_start_index..edge_results_end_index
#	puts edge_results_table

	map = Hash.new

	edge_results_table.each do |aString|

		splitted = aString.split "|"
			
		analysis_data = EdgeData.new
		analysis_data.flow = splitted[3].to_f
		analysis_data.velocity = splitted[4].to_f
		
		map[splitted[1]] = analysis_data
	end

	return map
end


def test

	map_of_first_analysis = do_one_analysis
	map_of_second_analysis = do_one_analysis

#	puts map_of_first_analysis

	combined_map = Hash.new

	map_of_second_analysis.each do |key,value|
	
		inner_map = Hash.new
		inner_map[:flow1] = map_of_first_analysis[key].flow
		inner_map[:flow2] = value.flow 
		inner_map[:flow_diff] = value.flow - map_of_first_analysis[key].flow

		inner_map[:velocity1] = map_of_first_analysis[key].velocity
		inner_map[:velocity2] = value.velocity
		inner_map[:velocity_diff] = value.velocity - map_of_first_analysis[key].velocity

		combined_map[key] = inner_map
	
	end

	org_string = ""
	org_string += "|edge_id|first flow|second flow|flow diff|first vel|snd vel|velocity comp|\n"
	org_string += "|-\n"

	combined_map.each do |key,value|

		org_string += "|#{key}|#{value[:flow1]}|#{value[:flow2]}|#{value[:flow_diff]}|#{value[:velocity1]}|#{value[:velocity2]}|#{value[:velocity_diff]}|\n"

	end	

	org_string += "|-\n"
	
	puts	org_string
end

test
