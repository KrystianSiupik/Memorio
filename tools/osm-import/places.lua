-- osm2pgsql flex style (MEM-E0-PLAT-03): named places from OpenStreetMap for place suggestions (C4).
-- The table holds OSM data unchanged and apart from the authors' own places (decision 34, L17, D-053):
-- ODbL applies to this table only. Ways are stored as their centroid.

local places = osm2pgsql.define_table({
    name = 'osm_places',
    schema = 'places',
    ids = { type = 'any', type_column = 'osm_type', id_column = 'osm_id' },
    columns = {
        { column = 'name', type = 'text', not_null = true },
        { column = 'kind', type = 'text', not_null = true },
        { column = 'tags', type = 'jsonb', not_null = true },
        { column = 'geom', type = 'point', projection = 4326, not_null = true },
    },
})

-- Keys that make a named object a place a memory can happen at, in order of preference.
local kinds = { 'place', 'tourism', 'natural', 'historic', 'leisure', 'amenity' }

local function kind_of(tags)
    for _, key in ipairs(kinds) do
        if tags[key] then
            return key .. '=' .. tags[key]
        end
    end
    return nil
end

local function row(object, geom)
    local kind = kind_of(object.tags)
    if not object.tags.name or not kind then
        return nil
    end
    return { name = object.tags.name, kind = kind, tags = object.tags, geom = geom }
end

function osm2pgsql.process_node(object)
    local place = row(object, object:as_point())
    if place then
        places:insert(place)
    end
end

function osm2pgsql.process_way(object)
    if not object.is_closed then
        return
    end
    local place = row(object, object:as_polygon():centroid())
    if place then
        places:insert(place)
    end
end
